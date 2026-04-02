using UnityEngine;
using Potato.Core;

namespace Potato.Gameplay
{
    [CreateAssetMenu(menuName = "ScriptableObjects/GameEvent<T>/Gameplay/WeaponAttackEvent")]
    public class WeaponAttackEvent : GameEvent<WeaponAttackInfo> {}
}
