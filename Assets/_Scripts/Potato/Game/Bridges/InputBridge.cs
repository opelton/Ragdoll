using UnityEngine;

namespace Potato.Game
{
    // todo -- input context should persist values through scene changes, but reset them when context changes
    public class InputBridge : MonoBehaviour, IInputPollingProvider
    {
        [SerializeField] private GameStateReference currentGameStateRef;

        bool IInputPollingProvider.GetButtonDown(string buttonName) => Input.GetButtonDown(buttonName);
        bool IInputPollingProvider.GetKeyDown(KeyCode key) => Input.GetKeyDown(key);
        float IInputPollingProvider.GetAxis(string axisName) => Input.GetAxisRaw(axisName);

        void Update() => currentGameStateRef.Value.Context.UpdateInputState(this);

        // clear stale inputs to prevent unintended onButtonUp events
        public void HandleGameStateChanged(GameState newState) => newState.Context.ResetInputStates();
    }
}