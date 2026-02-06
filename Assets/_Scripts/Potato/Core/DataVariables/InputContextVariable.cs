using System;
using UnityEngine;

namespace Potato.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DataVariables/InputContext")]
    public class InputContextVariable : DataVariable<InputContext> { }

    [Serializable]
    public class InputContextReference : DataReference<InputContextVariable, InputContext>
    {
        public InputContextReference() : base() { }
        public InputContextReference(InputContext value) : base(value) { }
        public InputContextReference(InputContextVariable referenceData) : base(referenceData) { }

        public static implicit operator InputContextReference(InputContext value) => new(value);
        public static implicit operator InputContextReference(InputContextVariable referenceData) => new(referenceData);
    }
}