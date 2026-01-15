using System;
using UnityEngine;

namespace Potato.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DataVariables/Int")]
    public class IntVariable : DataVariable<int> { }

    [Serializable]
    public class IntReference : DataReference<IntVariable, int>
    {
        public IntReference() : base() { }
        public IntReference(int value) : base(value) { }
        public IntReference(IntVariable referenceData) : base(referenceData) { }

        public static implicit operator IntReference(int value) => new(value);
        public static implicit operator IntReference(IntVariable referenceData) => new(referenceData);
    }
}