using UnityEngine;
using Potato.Core;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Input/Button")]
    public class InputButton : ScriptableObject
    {
        [SerializeField] private KeyCode keyCode;
        public KeyCode Key => keyCode;
        
#if UNITY_EDITOR
        [SerializeField] internal string _description;
#endif
        private bool _isDown = false;
        private bool _wasDown = false;

        public bool ButtonDown => _isDown;
        public bool ButtonPressed => _isDown && !_wasDown;  // down on this frame
        public bool ButtonReleased => !_isDown && _wasDown; // up on this frame

        [SerializeField] internal GameEvent onButtonPressed;
        [SerializeField] internal GameEvent onButtonReleased;

        public void UpdateState(bool isPressed)
        {
            _wasDown = _isDown;
            _isDown = isPressed;

            if (onButtonPressed != null && ButtonPressed)
                onButtonPressed.Invoke(this);

            else if (onButtonReleased != null && ButtonReleased)
                onButtonReleased.Invoke(this);
        }

        // initializes to current input state without invoking button pressed/released
        public void InitializeState(bool initialState = false)
        {
            _isDown = initialState;
            _wasDown = initialState;
        }
    }
}