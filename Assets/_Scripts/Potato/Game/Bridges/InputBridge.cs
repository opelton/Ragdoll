using Potato.Core;
using UnityEngine;

namespace Potato.Game
{
    public class InputBridge : MonoBehaviour, IInputPollingProvider
    {
        [SerializeField] private GameStateReference currentGameStateRef;
        [SerializeField] private BoolReference isPausedRef;

        bool IInputPollingProvider.GetButtonDown(string buttonName) => Input.GetButton(buttonName);
        bool IInputPollingProvider.GetKeyDown(KeyCode key) => Input.GetKey(key);
        float IInputPollingProvider.GetAxis(string axisName) => Input.GetAxisRaw(axisName);

        void Start() => ClearStaleButtonStates();

        void Update()
        {
            if(isPausedRef.Value)
                currentGameStateRef.Value.PauseContext.UpdateInputState(this);
            else
                currentGameStateRef.Value.Context.UpdateInputState(this);
        }

        public void ClearStaleButtonStates()
        {
            currentGameStateRef.Value.PauseContext.InitializeInputStates(this);
            currentGameStateRef.Value.Context.InitializeInputStates(this);
        }
    }
}