using UnityEngine;
using Potato.Core;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/GameEvent<T>/GameStateEvent")]
    public class GameStateEvent : GameEvent<GameState> {}
}
