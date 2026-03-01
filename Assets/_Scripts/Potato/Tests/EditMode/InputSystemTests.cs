using NUnit.Framework;
using Potato.Game;
using UnityEngine;
using Potato.Core;

namespace Potato.Tests.EditMode
{
    public class InputSystemTests
    {
        T MakeListener<T>(string name = "Listener")
            where T : Component
        {
            GameObject go = new(name);
            go.SetActive(false);
            return go.AddComponent<T>();
        }
        
        [Test]
        public void InputButtonInit()
        {
            InputButton button = ScriptableObject.CreateInstance<InputButton>();

            // tests that _isDown and _wasDown are both false
            Assert.IsFalse(button.ButtonDown);      // ButtonDown:      _isDown == true
            Assert.IsFalse(button.ButtonReleased);  // ButtonReleased:  _isDown == false && _wasDown == true
        }

        [Test]
        public void NoCrashOnNullEvents()
        {
            InputButton button = ScriptableObject.CreateInstance<InputButton>();

            // shouldn't crash when pushing buttons without callbacks
            Assert.DoesNotThrow(() =>
            {
                button.UpdateState(true);                
                button.UpdateState(false);
                button.UpdateState(true);
            });
        }

        [Test]
        public void PollingTest_Down()
        {
            InputButton button = ScriptableObject.CreateInstance<InputButton>();
            button.UpdateState(true);

            // ButtonDown: _isDown
            Assert.IsTrue(button.ButtonDown);
        }

        [Test]
        public void PollingTest_Pressed()
        {
            InputButton button = ScriptableObject.CreateInstance<InputButton>();

            button.UpdateState(false);
            button.UpdateState(true);

            // ButtonPressed: _isDown && !_wasDown
            Assert.IsTrue(button.ButtonPressed);
        }

        [Test]
        public void PollingTest_Released()
        {
            InputButton button = ScriptableObject.CreateInstance<InputButton>();

            button.UpdateState(true);
            button.UpdateState(false);

            // ButtonPressed: !_isDown && _wasDown
            Assert.IsTrue(button.ButtonReleased);
        }

        [Test]
        public void CallbackFires()
        {
            var listener = MakeListener<GameEventListener>();
            var dummyEvent = ScriptableObject.CreateInstance<GameEvent>();

            bool wasInvoked = false;
            listener.EventSource = dummyEvent;
            listener.Response.AddListener(() => wasInvoked = true);
            listener.gameObject.SetActive(true);

            InputButton button = ScriptableObject.CreateInstance<InputButton>();
            button.onButtonPressed = dummyEvent;

            button.UpdateState(true);
            Assert.IsTrue(wasInvoked);
        }

        [Test]
        public void NoCallbackOnButtonReset()
        {
            var listener = MakeListener<GameEventListener>();
            var dummyEvent = ScriptableObject.CreateInstance<GameEvent>();

            bool wasInvoked = false;
            listener.EventSource = dummyEvent;
            listener.Response.AddListener(() => wasInvoked = true);
            listener.gameObject.SetActive(true);

            InputButton button = ScriptableObject.CreateInstance<InputButton>();
            button.onButtonPressed = dummyEvent;

            // reset state sets button state without triggering callbacks
            button.ResetState(true);
            Assert.IsFalse(wasInvoked);
            Assert.IsTrue(button.ButtonDown);
        }

        [Test]
        public void NoCallbackOnButtonHold()
        {
            var listener = MakeListener<GameEventListener>();
            var dummyEvent = ScriptableObject.CreateInstance<GameEvent>();

            bool wasInvoked = false;
            listener.EventSource = dummyEvent;
            listener.Response.AddListener(() => wasInvoked = true);
            listener.gameObject.SetActive(true);

            InputButton button = ScriptableObject.CreateInstance<InputButton>();
            button.onButtonPressed = dummyEvent;

            // start in the down state
            button.ResetState(true);

            // ncontinuing to hold should not invoke ButtonPressed
            button.UpdateState(true);
            button.UpdateState(true);

            Assert.IsFalse(wasInvoked);
        }

        [Test]
        public void IntAxisInputButtonInit()
        {
            InputIntAxis axis = new();
            Assert.AreEqual(Vector2Int.zero, axis.Value);
        }

        [Test]
        public void IntAxisNoCrashOnNullEvents()
        {
            InputIntAxis axis = new();

            // shouldn't crash when pushing buttons without callbacks
            Assert.DoesNotThrow(() =>
            {
                axis.UpdateState(true, false, false, false);
                axis.UpdateState(false, false, true, false);
            });
        }

        [Test]
        public void IntAxisPollingTest()
        {
            InputIntAxis axis = new();
            axis.UpdateState(true, false, true, false);

            Assert.AreEqual(new Vector2Int(-1, 1), axis.Value);
        }

        [Test]
        public void IntAxisCallbackFires()
        {
            var listener = MakeListener<Vec2IntEventListener>();
            var dummyEvent = ScriptableObject.CreateInstance<Vec2IntEvent>();

            bool wasInvoked = false;
            listener.EventSource = dummyEvent;
            listener.Response.AddListener(_ => wasInvoked = true);
            listener.gameObject.SetActive(true);

            InputIntAxis axis = ScriptableObject.CreateInstance<InputIntAxis>();
            axis.onAxisChanged = dummyEvent;
            axis.UpdateState(false, false, true, false);

            Assert.IsTrue(wasInvoked);
        }

        [Test]
        public void IntAxisCallbackPayload()
        {
            Vector2Int outValue = Vector2Int.zero;
            var listener = MakeListener<Vec2IntEventListener>();
            var dummyEvent = ScriptableObject.CreateInstance<Vec2IntEvent>();

            listener.EventSource = dummyEvent;
            listener.Response.AddListener(value => outValue = value);
            listener.gameObject.SetActive(true);

            InputIntAxis axis = ScriptableObject.CreateInstance<InputIntAxis>();
            axis.onAxisChanged = dummyEvent;
            axis.UpdateState(false, false, true, false);

            Assert.AreEqual(Vector2Int.up, outValue);
        }

        [Test]
        public void IntAxisNoCallbackWithoutChange()
        {
            var listener = MakeListener<Vec2IntEventListener>();
            var dummyEvent = ScriptableObject.CreateInstance<Vec2IntEvent>();

            int count = 0;
            listener.EventSource = dummyEvent;
            listener.Response.AddListener(_ => ++count);
            listener.gameObject.SetActive(true);

            InputIntAxis axis = ScriptableObject.CreateInstance<InputIntAxis>();
            axis.onAxisChanged = dummyEvent;

            // both inputs the same, only one callback should happen
            axis.UpdateState(false, false, true, false);
            axis.UpdateState(false, false, true, false);

            Assert.AreEqual(1, count);
        }

        [Test]
        public void FloatAxisInputButtonInit()
        {
            InputFloatAxis axis = ScriptableObject.CreateInstance<InputFloatAxis>();
            Assert.AreEqual(Vector2.zero, axis.Value);
        }

        [Test]
        public void FloatAxisNoCrashOnNullEvents()
        {
            InputFloatAxis axis = ScriptableObject.CreateInstance<InputFloatAxis>();

            // shouldn't crash when pushing buttons without callbacks
            Assert.DoesNotThrow(() =>
            {
                axis.UpdateState(0f, 0f);
                axis.UpdateState(1f, 1f);
            });
        }

        [Test]
        public void FloatAxisPollingTest()
        {
            InputFloatAxis axis = ScriptableObject.CreateInstance<InputFloatAxis>();
            axis.UpdateState(1f, -1f);

            Assert.AreEqual(new Vector2(1f, -1f), axis.Value);
        }

        [Test]
        public void FloatAxisCallbackFires()
        {
            var listener = MakeListener<Vec2EventListener>();
            var dummyEvent = ScriptableObject.CreateInstance<Vec2Event>();

            bool wasInvoked = false;
            listener.EventSource = dummyEvent;
            listener.Response.AddListener(_ => wasInvoked = true);
            listener.gameObject.SetActive(true);

            InputFloatAxis axis = ScriptableObject.CreateInstance<InputFloatAxis>();
            axis.onAxisChanged = dummyEvent;
            axis.UpdateState(1f, -1f);

            Assert.IsTrue(wasInvoked);
        }

        [Test]
        public void FloatAxisCallbackPayload()
        {
            var listener = MakeListener<Vec2EventListener>();
            var dummyEvent = ScriptableObject.CreateInstance<Vec2Event>();

            Vector2 outValue = Vector2.zero;
            listener.EventSource = dummyEvent;
            listener.Response.AddListener(value => outValue = value);
            listener.gameObject.SetActive(true);

            InputFloatAxis axis = ScriptableObject.CreateInstance<InputFloatAxis>();
            axis.onAxisChanged = dummyEvent;
            axis.UpdateState(1f, -1f);

            Assert.AreEqual(new Vector2(1f, -1f), outValue);
        }

        [Test]
        public void FloatAxisNoCallbackWithoutChange()
        {
            var listener = MakeListener<Vec2EventListener>();
            var dummyEvent = ScriptableObject.CreateInstance<Vec2Event>();

            int count = 0;
            listener.EventSource = dummyEvent;
            listener.Response.AddListener(_ => ++count);
            listener.gameObject.SetActive(true);

            InputFloatAxis axis = ScriptableObject.CreateInstance<InputFloatAxis>();
            axis.onAxisChanged = dummyEvent;

            // both inputs the same, only one callback should happen
            axis.UpdateState(1f, 0f);
            axis.UpdateState(1f, 0f);

            Assert.AreEqual(1, count);
        }
    }
}