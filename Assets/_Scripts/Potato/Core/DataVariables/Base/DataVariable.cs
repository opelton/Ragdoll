using UnityEngine;

namespace Potato.Core
{
    // todo -- set/reset methods using readonly guard and triggering onchanged
    // holds the variable where it can be referenced in the editor
    public abstract class DataVariable<T> : DataVariableBase
    {
        [SerializeField] private T _initialValue = default;
        [SerializeField] private T _value;
        [SerializeField] internal GameEvent<T> onValueChanged;
        public T InitialValue { get => _initialValue; set => TrySetInitialValue(value); }
        public T Value { get => _value; set => TrySetValue(value); } 

        internal override void ResetValue() => SetValueAndNotify(_initialValue);
        public override void MakeReadonly()
        {
            base.MakeReadonly();
            _value = _initialValue;
        }

        // refuses the change if _isReadonly
        void TrySetValue(T newValue)
        {
            if (CheckReadonlyAndLogWarning())
                SetValueAndNotify(newValue);
        }

        void TrySetInitialValue(T newInitialValue)
        {
            if (CheckReadonlyAndLogWarning())
                _initialValue = newInitialValue;
        }

        bool CheckReadonlyAndLogWarning()
        {
#if UNITY_EDITOR
            if (_isReadonly)
            {
                Debug.LogWarning(
                    $"Attempted to modify const DataVariable '{name}'. Value unchanged.",
                    this);
            }
#endif
            return !_isReadonly;
        }

        void SetValueAndNotify(T newValue)
        {
            _value = newValue;

            if(onValueChanged)
                onValueChanged.Invoke(_value, this);
        }


#if UNITY_EDITOR
        internal override object GetValue() => Value;
        internal override void SetValue(object valueObj) => Value = (T)valueObj;
        internal override void SetInitialValue(object initialValueObj) => InitialValue = (T)initialValueObj;
        internal override object ValueObject { get => Value; set => Value = (T)value; }
        internal override object InitialValueObject { get => InitialValue; set => InitialValue = (T)value; }
#endif
    }
}