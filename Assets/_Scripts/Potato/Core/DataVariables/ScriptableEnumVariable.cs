using System;
using UnityEngine;

namespace Potato.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DataVariables/ScriptableEnum")]
    public class ScriptableEnumVariable : DataVariable<ScriptableEnum> { }

    [Serializable]
    public class ScriptableEnumReference : DataReference<ScriptableEnumVariable, ScriptableEnum>
    {
        public ScriptableEnumReference() : base() { }
        public ScriptableEnumReference(ScriptableEnum value) : base(value) { }
        public ScriptableEnumReference(ScriptableEnumVariable referenceData) : base(referenceData) { }

        public static implicit operator ScriptableEnumReference(ScriptableEnum value) => new(value);
        public static implicit operator ScriptableEnumReference(ScriptableEnumVariable referenceData) => new(referenceData);
    }
}