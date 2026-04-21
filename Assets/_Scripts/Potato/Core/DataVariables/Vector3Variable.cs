using System;
using UnityEngine;

namespace Potato.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DataVariables/Vector3")]
    public class Vector3Variable : DataVariable<Vector3> { }

    [Serializable]
    public class Vector3Reference : DataReference<Vector3Variable, Vector3>
    {
        public Vector3Reference() : base() { }
        public Vector3Reference(Vector3 value) : base(value) { }
        public Vector3Reference(Vector3Variable referenceData) : base(referenceData) { }

        public static implicit operator Vector3Reference(Vector3 value) => new(value);
        public static implicit operator Vector3Reference(Vector3Variable referenceData) => new(referenceData);
    }
}