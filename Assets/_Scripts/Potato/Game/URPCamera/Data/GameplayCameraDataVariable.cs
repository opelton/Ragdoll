using System;
using UnityEngine;
using Potato.Core;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DataVariables/GameplayCameraData")]
    public class GameplayCameraDataVariable : DataVariable<GameplayCameraData> { }

    [Serializable]
    public class GameplayCameraDataReference : DataReference<GameplayCameraDataVariable, GameplayCameraData>
    {
        public GameplayCameraDataReference() : base() { }
        public GameplayCameraDataReference(GameplayCameraDataVariable referenceData) : base(referenceData) { }
        public static implicit operator GameplayCameraDataReference(GameplayCameraDataVariable referenceData) => new(referenceData);
    }
}