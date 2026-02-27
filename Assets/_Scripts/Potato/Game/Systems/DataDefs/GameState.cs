using UnityEngine;
using Potato.Core;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Enums/GameState")]
    public class GameState : ScriptableEnum
    {
        [SerializeField] private bool cursorLocked;
        [SerializeField] private InputContext context;
        // hacky way to include a quit button in the same panel that appears on the main menu
        [SerializeField] private bool settingsPanelMode;

        public bool CursorLocked => cursorLocked;
        public InputContext Context => context;
        public bool SettingsPanelMode => settingsPanelMode;
    }
}