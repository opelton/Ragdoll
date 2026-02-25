using NUnit.Framework;
using Potato.Game;
using UnityEngine;

namespace Potato.Tests.EditMode
{
    public class InputSystemTests
    {
        [Test]
        public void InputButtonInit()
        {
            InputButton button = new();

            // tests that _isDown and _wasDown are both false
            Assert.IsFalse(button.ButtonDown);      // ButtonDown:      _isDown == true
            Assert.IsFalse(button.ButtonReleased);  // ButtonReleased:  _isDown == false && _wasDown == true
        }

        [Test]
        public void NoCrashOnNullEvents()
        {
            InputButton button = new();

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
            InputButton button = new();
            button.UpdateState(true);

            // ButtonDown: _isDown
            Assert.IsTrue(button.ButtonDown);
        }

        [Test]
        public void PollingTest_Pressed()
        {
            InputButton button = new();

            button.UpdateState(false);
            button.UpdateState(true);

            // ButtonPressed: _isDown && !_wasDown
            Assert.IsTrue(button.ButtonPressed);
        }

        [Test]
        public void PollingTest_Released()
        {
            InputButton button = new();

            button.UpdateState(true);
            button.UpdateState(false);

            // ButtonPressed: !_isDown && _wasDown
            Assert.IsTrue(button.ButtonReleased);
        }

        [Test]
        public void CallbackFires()
        {
            bool wasFired = false;
            InputButton button = new();
            button.OnButtonPressed += () => wasFired = true;
            button.UpdateState(true);

            Assert.IsTrue(wasFired);
        }

        [Test]
        public void NoCallbackOnButtonHold()
        {
            bool wasFired = false;
            InputButton button = new();

            // set past and prev state so the button is in "hold" state
            button.UpdateState(true);
            button.UpdateState(true);

            button.OnButtonPressed += () => wasFired = true;

            // no events should fire while hold state continuing
            button.UpdateState(true);
            button.UpdateState(true);

            Assert.IsFalse(wasFired);
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
            bool wasFired = false;
            InputIntAxis axis = new();
            axis.OnAxisChanged += value => wasFired = true;
            axis.UpdateState(false, false, true, false);

            Assert.IsTrue(wasFired);
        }

        [Test]
        public void IntAxisCallbackPayload()
        {
            Vector2Int outValue = Vector2Int.zero;
            InputIntAxis axis = new();
            axis.OnAxisChanged += value => outValue = value;
            axis.UpdateState(false, false, true, false);

            Assert.AreEqual(Vector2Int.up, outValue);
        }

        [Test]
        public void IntAxisNoCallbackWithoutChange()
        {
            bool wasFired = false;
            InputIntAxis axis = new();
            axis.UpdateState(false, false, true, false);
            axis.OnAxisChanged += value => wasFired = true;
            axis.UpdateState(false, false, true, false);

            Assert.IsFalse(wasFired);
        }

        [Test]
        public void FloatAxisInputButtonInit()
        {
            InputFloatAxis axis = new();
            Assert.AreEqual(Vector2.zero, axis.Value);
        }

        [Test]
        public void FloatAxisNoCrashOnNullEvents()
        {
            InputFloatAxis axis = new();

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
            InputFloatAxis axis = new();
            axis.UpdateState(1f, -1f);

            Assert.AreEqual(new Vector2(1f, -1f), axis.Value);
        }

        [Test]
        public void FloatAxisCallbackFires()
        {
            bool wasFired = false;
            InputFloatAxis axis = new();
            axis.OnAxisChanged += value => wasFired = true;
            axis.UpdateState(1f, -1f);

            Assert.IsTrue(wasFired);
        }

        [Test]
        public void FloatAxisCallbackPayload()
        {
            Vector2 outValue = Vector2.zero;
            InputFloatAxis axis = new();
            axis.OnAxisChanged += value => outValue = value;
            axis.UpdateState(1f, -1f);

            Assert.AreEqual(new Vector2(1f, -1f), outValue);
        }

        [Test]
        public void FloatAxisNoCallbackWithoutChange()
        {
            bool wasFired = false;
            InputFloatAxis axis = new();
            axis.UpdateState(1f, -1f);
            axis.OnAxisChanged += value => wasFired = true;
            axis.UpdateState(1f, -1f);

            Assert.IsFalse(wasFired);
        }
    }
}