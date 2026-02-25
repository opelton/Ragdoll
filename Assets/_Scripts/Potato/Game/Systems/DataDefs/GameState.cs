using UnityEngine;
using Potato.Core;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Enums/GameState")]
    public class GameState : ScriptableEnum
    {
        [SerializeField] private bool cursorLocked;
        [SerializeField] private InputContext context;

        public bool CursorLocked => cursorLocked;
        public InputContext Context => context;
    }
}