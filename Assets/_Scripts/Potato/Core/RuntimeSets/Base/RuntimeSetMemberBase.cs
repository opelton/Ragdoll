using UnityEngine;

namespace Potato.Core
{
    // adds and removes self from the runtime set
    public abstract class RuntimeSetMemberBase : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] internal string _description;
        internal abstract object GetValue();
#endif
    }
}