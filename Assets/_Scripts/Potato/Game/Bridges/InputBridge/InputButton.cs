using System;
using UnityEngine;

namespace Potato.Game
{
    public class InputButton
    {
        private bool _isDown = false;
        private bool _wasDown = false;

        public bool ButtonDown => _isDown;
        public bool ButtonPressed => _isDown && !_wasDown;  // down on this frame
        public bool ButtonReleased => !_isDown && _wasDown; // up on this frame

        public event Action OnButtonPressed;
        public event Action OnButtonReleased;

        public InputButton() => ResetState();

        public void UpdateState(bool isPressed)
        {
            _wasDown = _isDown;
            _isDown = isPressed;

            // Dispatch events based on state transitions
            if (_isDown && !_wasDown)
                OnButtonPressed?.Invoke();

            else if (!_isDown && _wasDown)
                OnButtonReleased?.Invoke();
        }

        public void ResetState()
        {
            _isDown = false;
            _wasDown = false;
        }
    }

    public class InputIntAxis
    {
        public Vector2Int Value { get; private set;} = Vector2Int.zero;
        public event Action<Vector2Int> OnAxisChanged;

        public void UpdateState(bool left, bool right, bool up, bool down)
        {
            Vector2Int newAxis = Vector2Int.zero;

            if(right)   newAxis.x += 1;
            if(left)    newAxis.x -= 1;
            if(up)      newAxis.y += 1;
            if(down)    newAxis.y -= 1;

            if(OnAxisChanged != null && Value != newAxis)
                OnAxisChanged.Invoke(newAxis);
            
            Value = newAxis;
        }

        public void UpdateState(float horizontal, float vertical, float deadzone = .2f)
        {
            Vector2Int newAxis = Vector2Int.zero;
            if(horizontal > deadzone)       newAxis.x = 1;
            else if(horizontal < -deadzone) newAxis.x = -1;
            if(vertical > deadzone)         newAxis.y = 1;
            else if(vertical < -deadzone)   newAxis.y = -1;

            if(OnAxisChanged != null && Value != newAxis)
                OnAxisChanged.Invoke(newAxis);
            
            Value = newAxis;
        }

        public void ResetState()
        {
            Value = Vector2Int.zero;
        }
    }

    public class InputFloatAxis
    {
        public Vector2 Value { get; private set;} = Vector2.zero;
        public event Action<Vector2> OnAxisChanged;

        public void UpdateState(float horizontal, float vertical)
        {
            Vector2 newAxis = new (horizontal, vertical);

            if(OnAxisChanged != null && Value != newAxis)
                OnAxisChanged.Invoke(newAxis);
            
            Value = newAxis;
        }

        public void ResetState()
        {
            Value = Vector2.zero;
        }
    }
}