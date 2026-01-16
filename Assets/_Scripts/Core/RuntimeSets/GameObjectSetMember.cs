using UnityEngine;

namespace Potato.Core
{
    public class GameObjectSetMember : RuntimeSetMember<GameObject>
    {
        protected override GameObject Value => gameObject;
    }
}