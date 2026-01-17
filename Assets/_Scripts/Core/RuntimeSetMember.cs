using UnityEngine;

namespace Potato.Core
{
    // adds and removes self from the runtime set
    public abstract class RuntimeSetMember<T> : RuntimeSetMemberBase
    {
        [SerializeField] protected RuntimeSet<T> runtimeSet;

        // if T isn't a component type, make sure to override T Value get;
        protected virtual T Value => gameObject.GetComponent<T>();

        protected virtual void OnEnable()
        {
            if(runtimeSet != null)
                runtimeSet.Add(Value);
        }
        
        protected virtual void OnDisable()
        {
            if(runtimeSet != null)
                runtimeSet.Remove(Value);
        }

#if UNITY_EDITOR
        internal override object GetValue() => Value;
#endif
    }
}