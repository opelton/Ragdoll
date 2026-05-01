using UnityEngine;
using Potato.Core;

namespace Potato.Gameplay
{
    [CreateAssetMenu(menuName = "ScriptableObjects/GameEvent<T>/PlayerCharacterControllerEvent")]
    public class PlayerCharacterControllerEvent : GameEvent<PlayerCharacterController> {}
}
