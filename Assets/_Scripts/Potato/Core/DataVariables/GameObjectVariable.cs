using System;
using UnityEngine;

namespace Potato.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DataVariables/GameObject")]
    public class GameObjectVariable : DataVariable<GameObject> { }

    [Serializable]
    public class GameObjectReference : DataReference<GameObjectVariable, GameObject>
    {
        public GameObjectReference() : base() { }
        public GameObjectReference(GameObject value) : base(value) { }
        public GameObjectReference(GameObjectVariable referenceData) : base(referenceData) { }

        public static implicit operator GameObjectReference(GameObject value) => new(value);
        public static implicit operator GameObjectReference(GameObjectVariable referenceData) => new(referenceData);
    }
}