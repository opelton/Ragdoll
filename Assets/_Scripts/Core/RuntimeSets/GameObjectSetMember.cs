using UnityEngine;

namespace Potato.Core
{
    public class GameObjectSetMember : RuntimeSetMember<GameObject>
    {
        protected override GameObject Value => gameObject;

        protected override void OnEnable() => runtimeSet.Add(Value);
        protected override void OnDisable() => runtimeSet.Remove(Value);
    }
}