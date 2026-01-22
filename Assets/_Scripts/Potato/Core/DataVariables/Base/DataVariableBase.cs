using UnityEngine;

namespace Potato.Core
{
    // non-generic base class exposes members for unit testing and UI
    public abstract class DataVariableBase : ScriptableObject, IPreInit
    {
        [SerializeField] protected bool _isReadonly = false;

        public virtual void SetReadonly(bool isReadonly, bool forceUpdateValue = true) => _isReadonly = isReadonly;
        public abstract void ResetValue();
        public void PreInit() => ResetValue();

#if UNITY_EDITOR
        [SerializeField] protected string _description;
        internal abstract object GetValue();
        internal abstract void SetValue(object valueObj);
        internal abstract void SetInitialValue(object initialValueObj);
        internal abstract object ValueObject { get; }
        internal abstract object InitialValueObject { get; }
        internal abstract void SetValueProperty(object valueObj);
#endif
    }
}