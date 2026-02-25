using UnityEngine;
using Potato.Core;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/GameEvent<T>/InputContextEvent")]
    public class InputContextEvent : GameEvent<InputContext> {}
}
