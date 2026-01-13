using UnityEngine;

namespace Potato.Core
{
    // adds and removes self from the runtime set, this just saves a tiny bit of time coding it manually
    public abstract class RuntimeSetMember<T> : MonoBehaviour
    {
        [SerializeField] protected RuntimeSet<T> runtimeSet;
        protected virtual T Value => gameObject.GetComponent<T>();


        protected virtual void OnEnable() => runtimeSet.Add(Value);
        protected virtual void OnDisable() => runtimeSet.Remove(Value);
    }
}