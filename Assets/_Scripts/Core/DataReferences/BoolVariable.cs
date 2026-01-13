using System;
using UnityEngine;

namespace Potato.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DataVariables/Bool")]
    public class BoolVariable : DataVariable<bool> { }

    [Serializable]
    public class BoolReference : DataReference<BoolVariable, bool>
    {
        public BoolReference(bool value) : base(value) { }
        public BoolReference(BoolVariable referenceData) : base(referenceData) { }

        public static implicit operator BoolReference(bool value) => new(value);
        public static implicit operator BoolReference(BoolVariable referenceData) => new(referenceData);
    }
}