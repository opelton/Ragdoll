using System;
using UnityEngine;

namespace Potato.Core
{
    [Serializable]
    public abstract class DataReferenceBase
    {
        public bool UseConstant = false;
        public DataReferenceBase() { }
        public abstract void ClearReference();

#if UNITY_EDITOR
        [SerializeField] internal string _description;
        internal abstract object GetValue();
        internal abstract void SetValue(object valueObject);
        internal abstract object GetReference();
        internal abstract void SetReference(object referenceObj);
#endif
    }
}