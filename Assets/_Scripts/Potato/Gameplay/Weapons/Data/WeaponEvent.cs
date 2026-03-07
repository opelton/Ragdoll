using UnityEngine;
using Potato.Core;

namespace Potato.Gameplay
{
    [CreateAssetMenu(menuName = "ScriptableObjects/GameEvent<T>/Gameplay/WeaponEvent")]
    public class WeaponEvent : GameEvent<WeaponController> {}
}
