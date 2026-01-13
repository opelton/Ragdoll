using System;

namespace Potato.Core
{
    // treat another scriptable object as a variable in a script, inject it from the editor, keep things nicely separated
    [Serializable]
    public abstract class DataReference<T, U> where T : DataVariable<U>
    {
        public bool UseConstant = true;
        public U ConstantValue;
        public T ReferenceData;

        protected DataReference() { }

        protected DataReference(U value)
        {
            UseConstant = true;
            ConstantValue = value;
        }

        protected DataReference(T referenceData)
        {
            UseConstant = false;
            ReferenceData = referenceData;
        }

        public U Value { get { return UseConstant ? ConstantValue : ReferenceData.Value; } }
    }
}