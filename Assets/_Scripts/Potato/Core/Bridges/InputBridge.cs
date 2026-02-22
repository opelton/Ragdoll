using UnityEngine;

namespace Potato.Core
{
    // todo -- input context should persist values through scene changes, but reset them when context changes
    public class InputBridge : MonoBehaviour, IInputPollingProvider
    {
        // todo -- set this using the rest of the input system when it's done (bootstrap config injects signal to request context?) instead of injecting it directly
        [Tooltip("assign a starting default")]
        [SerializeField] private InputContext _currentInputContext;

        bool IInputPollingProvider.GetButtonDown(string buttonName) => Input.GetButtonDown(buttonName);
        bool IInputPollingProvider.GetKeyDown(KeyCode key) => Input.GetKeyDown(key);
        float IInputPollingProvider.GetAxis(string axisName) => Input.GetAxisRaw(axisName);

        void Update() => _currentInputContext.UpdateInputState(this);

        public void HandleInputContextChanged(InputContext newContext)
        {
            // contexts are reusable, resetting their state prevents orphaned motion and keyUp events on reenable
            if(_currentInputContext != null)
                _currentInputContext.ResetInputStates();
            
            _currentInputContext = newContext;
        }
    }
}