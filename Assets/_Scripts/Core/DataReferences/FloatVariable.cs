using System;
using UnityEngine;

namespace Potato.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DataVariables/Float")]
    public class FloatVariable : DataVariable<float> { }

    [Serializable]
    public class FloatReference : DataReference<FloatVariable, float>
    {
        public FloatReference(float value) : base(value) { }
        public FloatReference(FloatVariable referenceData) : base(referenceData) { }

        public static implicit operator FloatReference(float value) => new(value);
        public static implicit operator FloatReference(FloatVariable referenceData) => new(referenceData);
    }
}