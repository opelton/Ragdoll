using UnityEngine;

namespace Potato.Core
{
    // adds and removes self from the runtime set
    public abstract class RuntimeSetMember<T> : MonoBehaviour
    {
        [SerializeField] protected RuntimeSet<T> runtimeSet;
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
    }
}