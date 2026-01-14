using System;
using UnityEngine;

namespace Potato.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DataVariables/String")]
    public class StringVariable : DataVariable<string> { }

    [Serializable]
    public class StringReference : DataReference<StringVariable, string>
    {
        public StringReference(string value) : base(value) { }
        public StringReference(StringVariable referenceData) : base(referenceData) { }

        public static implicit operator StringReference(string value) => new(value);
        public static implicit operator StringReference(StringVariable referenceData) => new(referenceData);
    }
}