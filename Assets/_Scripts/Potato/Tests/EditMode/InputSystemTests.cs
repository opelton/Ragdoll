using NUnit.Framework;
using Potato.Core;
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
        public void AxisInputButtonInit()
        {
            InputAxisButtons axis = new();
            Assert.AreEqual(Vector2Int.zero, axis.Value);
        }

        [Test]
        public void AxisNoCrashOnNullEvents()
        {
            InputAxisButtons axis = new();

            // shouldn't crash when pushing buttons without callbacks
            Assert.DoesNotThrow(() =>
            {
                axis.UpdateState(true, false, false, false);
                axis.UpdateState(false, false, true, false);
            });
        }

        [Test]
        public void AxisPollingTest()
        {
            InputAxisButtons axis = new();
            axis.UpdateState(true, false, true, false);

            Assert.AreEqual(new Vector2Int(-1, 1), axis.Value);
        }

        [Test]
        public void AxisCallbackFires()
        {
            bool wasFired = false;
            InputAxisButtons axis = new();
            axis.OnAxisChanged += value => wasFired = true;
            axis.UpdateState(false, false, true, false);

            Assert.IsTrue(wasFired);
        }

        [Test]
        public void AxisCallbackPayload()
        {
            Vector2Int outValue = Vector2Int.zero;
            InputAxisButtons axis = new();
            axis.OnAxisChanged += value => outValue = value;
            axis.UpdateState(false, false, true, false);

            Assert.AreEqual(Vector2Int.up, outValue);
        }
    }
}