using System;
using UnityEngine;
using Potato.Core;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DataVariables/GameState")]
    public class GameStateVariable : DataVariable<GameState> { }

    [Serializable]
    public class GameStateReference : DataReference<GameStateVariable, GameState>
    {
        public GameStateReference() : base() { }
        public GameStateReference(GameState value) : base(value) { }
        public GameStateReference(GameStateVariable referenceData) : base(referenceData) { }

        public static implicit operator GameStateReference(GameState value) => new(value);
        public static implicit operator GameStateReference(GameStateVariable referenceData) => new(referenceData);
    }
}