using UnityEngine;

namespace Potato.Core
{
    // non-generic base class exposes members for unit testing and UI
    public abstract class DataVariableBase : ScriptableObject, IPreInit
    {
        [SerializeField] protected bool _isReadonly = false;

        // for runtime readonly
        public virtual void MakeReadonly() => _isReadonly = true;
        internal abstract void ResetValue();
        public void PreInit() => ResetValue();

#if UNITY_EDITOR
        [SerializeField] protected string _description;
        internal abstract object GetValue();
        internal abstract void SetValue(object valueObj);
        internal abstract void SetInitialValue(object initialValueObj);
        internal abstract object ValueObject { get; set; }
        internal abstract object InitialValueObject { get; set; }
#endif
    }
}