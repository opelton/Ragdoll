using UnityEngine;
using Potato.Core;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Enums/GameState")]
    public class GameState : ScriptableEnum
    {
        [SerializeField] private bool cursorLock_Paused;
        [SerializeField] private bool cursorLock_Unpaused;
        [SerializeField] private InputContext context;
        [SerializeField] private InputContext pauseContext;
        // hacky way to include a quit button in the same panel that appears on the main menu
        [SerializeField] private bool showGameFlowSettingsButtons;

        public bool CursorLockWhilePaused => cursorLock_Paused;
        public bool CursorLockWhileUnpaused => cursorLock_Unpaused;
        public InputContext Context => context;
        public InputContext PauseContext => pauseContext;
        public bool ShowGameFlowSettingsButtons => showGameFlowSettingsButtons;
    }
}