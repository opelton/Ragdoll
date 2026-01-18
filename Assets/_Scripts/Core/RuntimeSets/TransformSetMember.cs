using UnityEngine;

namespace Potato.Core
{
    public class TransformSetMember : RuntimeSetMember<Transform>
    {
        internal override Transform Value => transform;
    }
}