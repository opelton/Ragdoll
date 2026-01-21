using System;

namespace Potato.Core
{
    [Serializable]
    public abstract class DataReferenceBase
    {
        public bool UseConstant = true;
        public DataReferenceBase() { }
        public abstract void ClearReference();

#if UNITY_EDITOR
        internal abstract object GetValue();
        internal abstract void SetValue(object valueObject);
        internal abstract object GetReference();
        internal abstract void SetReference(object referenceObj);
#endif
    }
}