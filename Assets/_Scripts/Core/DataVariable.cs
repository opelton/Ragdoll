using UnityEngine;

namespace Potato.Core
{
    // holds the variable where it can be referenced in the editor
    public abstract class DataVariable<T> : DataVariableBase
    {
        [SerializeField] private T _initialValue = default;
        [SerializeField] private T _value;
        public T InitialValue { get { return _initialValue; } }
        public T Value { get => _value; set => CheckReadonlyGuard(value); } // CheckReadonlyGuard refuses the change if _isReadonly

        public void SetValue(T value) => _value = value;
        public void SetInitialValue(T initialValue) => _initialValue = initialValue;
        public override void ResetValue() => _value = _initialValue;
        public override void SetReadonly(bool isReadonly, bool forceUpdateValue = true)
        {
            base.SetReadonly(isReadonly);
            if (_isReadonly && forceUpdateValue)
                _value = _initialValue;
        }

        void CheckReadonlyGuard(T newValue)
        {
            if (_isReadonly)
            {
#if UNITY_EDITOR
                Debug.LogWarning(
                    $"Attempted to modify const DataVariable '{name}'. Value unchanged.",
                    this);
#endif
                return;
            }
            _value = newValue;
        }


#if UNITY_EDITOR
        public override object GetValue() => Value;
        public override void SetValue(object valueObj) => SetValue((T)valueObj);
        public override void SetInitialValue(object initialValueObj) => SetInitialValue((T)initialValueObj);
        internal override object ValueObject { get => _value; }
        internal override object InitialValueObject { get => _initialValue; }
        internal override void SetValueProperty(object valueObj) => Value = (T)valueObj;
#endif
    }
}