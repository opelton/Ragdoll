using System;
using System.Collections.Generic;

namespace Potato.Core
{
    public class Bindable<T>
    {
        private T _value;
        public event Action<T> OnValueChanged;

        public T Value
        {
            get => _value;
            set => SetValue(value);
        }

        void SetValue(T newValue)
        {
            if(EqualityComparer<T>.Default.Equals(_value, newValue))
                return;
            
            _value = newValue;
            OnValueChanged?.Invoke(_value);
        }

        public Bindable(T initialValue = default)
        {
            _value = initialValue;
        }
    }
}