using UnityEngine;

namespace Potato.Game
{
    public class CursorBridge : MonoBehaviour
    {
        [SerializeField] private GameStateReference currentGameStateRef;

        void Start() => SetCursorLockState(currentGameStateRef.Value.CursorLocked);

        public void SetCursorLockState(bool isLocked)
        {
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isLocked;
        }

        public void OnGameStateChanged(GameState newGameState) => SetCursorLockState(newGameState.CursorLocked);

        public void OnPauseStateChanged(bool isPaused) => SetCursorLockState(!isPaused);
    }
}