using System;
using UnityEngine;

namespace Potato.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DataVariables/Transform")]
    public class TransformVariable : DataVariable<Transform> { }

    [Serializable]
    public class TransformReference : DataReference<TransformVariable, Transform>
    {
        public TransformReference(Transform value) : base(value) { }
        public TransformReference(TransformVariable referenceData) : base(referenceData) { }

        public static implicit operator TransformReference(Transform value) => new(value);
        public static implicit operator TransformReference(TransformVariable referenceData) => new(referenceData);
    }
}