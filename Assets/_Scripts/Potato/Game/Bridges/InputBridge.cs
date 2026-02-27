using Potato.Core;
using UnityEngine;

namespace Potato.Game
{
    // todo -- input context should persist values through scene changes, but reset them when context changes
    public class InputBridge : MonoBehaviour, IInputPollingProvider
    {
        [SerializeField] private GameStateReference currentGameStateRef;
        [SerializeField] private BoolReference isPausedRef;

        bool IInputPollingProvider.GetButtonDown(string buttonName) => Input.GetButtonDown(buttonName);
        bool IInputPollingProvider.GetKeyDown(KeyCode key) => Input.GetKeyDown(key);
        float IInputPollingProvider.GetAxis(string axisName) => Input.GetAxisRaw(axisName);

        void Start() => ClearStaleButtonStates();

        void Update()
        {
            if(isPausedRef.Value)
                currentGameStateRef.Value.PauseContext.UpdateInputState(this);
            else
                currentGameStateRef.Value.Context.UpdateInputState(this);
        }

        void ClearStaleButtonStates()
        {
            currentGameStateRef.Value.PauseContext.ResetInputStates();
            currentGameStateRef.Value.Context.ResetInputStates();
        }

        public void HandleGameStateChanged(GameState _) => ClearStaleButtonStates();
    }
}