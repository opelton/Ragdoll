using UnityEngine;

namespace Potato.Core
{
    public class GameObjectSetMember : RuntimeSetMember<GameObject>
    {
        internal override GameObject Value => gameObject;
    }
}