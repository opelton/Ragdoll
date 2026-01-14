using UnityEngine;

namespace Potato.Core
{
    // holds the variable where it can be referenced in the editor
    public abstract class DataVariable<T> : ScriptableObject
    {
        [SerializeField] private T _initialValue = default;
        public T Value = default;
        public T InitialValue { get { return _initialValue; } }

        public void SetValue(T value) => Value = value;
        public void SetInitialValue(T initialValue) => _initialValue = initialValue;
        public void ResetValue() => Value = _initialValue;
    }
}