using UnityEngine;

namespace Potato.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/GameEvent<T>/GameObjectEvent")]
    public class GameObjectEvent : GameEvent<GameObject> {}
}
