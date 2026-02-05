using System;

namespace Potato.Core
{
    // treat another scriptable object as a variable in a script, inject it from the editor, keep things nicely separated
    // todo -- runtime reference switching?
    [Serializable]
    public abstract class DataReference<T, U> : DataReferenceBase
        where T : DataVariable<U>
    {
        internal U ConstantValue = default;
        internal T ReferenceData;

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

        bool ShouldUseReferenceData => !UseConstant && ReferenceData != null;

        public U Value
        {
            get => ShouldUseReferenceData ? ReferenceData.Value : ConstantValue;
            set
            {
                if (ShouldUseReferenceData)
                    ReferenceData.Value = value;
                else
                    ConstantValue = value;
            }
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