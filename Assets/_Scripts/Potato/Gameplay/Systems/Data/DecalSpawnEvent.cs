using UnityEngine;
using Potato.Core;

namespace Potato.Gameplay
{
    [CreateAssetMenu(menuName = "ScriptableObjects/GameEvent<T>/DecalSpawnEvent")]
    public class DecalSpawnEvent : GameEvent<DecalSpawnData> {}
}
