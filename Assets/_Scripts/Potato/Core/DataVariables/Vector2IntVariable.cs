using System;
using UnityEngine;

namespace Potato.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DataVariables/Vector2Int")]
    public class Vector2IntVariable : DataVariable<Vector2Int> { }

    [Serializable]
    public class Vector2IntReference : DataReference<Vector2IntVariable, Vector2Int>
    {
        public Vector2IntReference() : base() { }
        public Vector2IntReference(Vector2Int value) : base(value) { }
        public Vector2IntReference(Vector2IntVariable referenceData) : base(referenceData) { }

        public static implicit operator Vector2IntReference(Vector2Int value) => new(value);
        public static implicit operator Vector2IntReference(Vector2IntVariable referenceData) => new(referenceData);
    }
}