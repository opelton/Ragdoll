using System;

namespace Potato.Core
{
    // treat another scriptable object as a variable in a script, inject it from the editor, keep things nicely separated
    [Serializable]
    public abstract class DataReference<T, U> : DataReferenceBase where T : DataVariable<U>
    {
        public U ConstantValue;
        public T ReferenceData;

        public DataReference() { }

        public DataReference(U value)
        {
            UseConstant = true;
            ConstantValue = value;
        }

        public DataReference(T referenceData)
        {
            UseConstant = false;
            ReferenceData = referenceData;
        }

        public override void ClearReference()
        {
            UseConstant = true;
            ReferenceData = null;
        }

        public U Value
        {
            get { return UseConstant ? ConstantValue : ReferenceData.Value; }
            set { if (UseConstant) ConstantValue = value; else ReferenceData.Value = value; }
        }

#if UNITY_EDITOR
        internal override object GetValue() => Value;
        internal override void SetValue(object valueObject) => Value = (U)valueObject;
        internal override object GetReference() => ReferenceData;
        internal override void SetReference(object referenceObj)
        {
            ReferenceData = (T)referenceObj;
            UseConstant = false;
        }
#endif
    }
}