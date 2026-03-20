using Potato.Core;
using UnityEngine;

namespace Potato.Game
{
    public class CursorBridge : MonoBehaviour
    {
        [SerializeField] private GameStateReference currentGameStateRef;
        [SerializeField] private BoolReference isPausedRef;

        void Start() => UpdateCursorLockState();

        void UpdateCursorLockState()
        {
            bool lockCursor = isPausedRef.Value 
                ? currentGameStateRef.Value.CursorLockWhilePaused 
                : currentGameStateRef.Value.CursorLockWhileUnpaused;

            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !lockCursor;
        }

        public void OnGameStateChanged(GameState _) => UpdateCursorLockState();

        public void OnPauseStateChanged(bool _) => UpdateCursorLockState();
    }
}