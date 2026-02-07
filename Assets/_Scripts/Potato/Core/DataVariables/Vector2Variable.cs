using System;
using UnityEngine;

namespace Potato.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DataVariables/Vector2")]
    public class Vector2Variable : DataVariable<Vector2> { }

    [Serializable]
    public class Vector2Reference : DataReference<Vector2Variable, Vector2>
    {
        public Vector2Reference() : base() { }
        public Vector2Reference(Vector2 value) : base(value) { }
        public Vector2Reference(Vector2Variable referenceData) : base(referenceData) { }

        public static implicit operator Vector2Reference(Vector2 value) => new(value);
        public static implicit operator Vector2Reference(Vector2Variable referenceData) => new(referenceData);
    }
}