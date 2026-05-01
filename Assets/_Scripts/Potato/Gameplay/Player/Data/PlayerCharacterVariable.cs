using System;
using UnityEngine;
using Potato.Core;

namespace Potato.Gameplay
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DataVariables/PlayerCharacterController")]
    public class PlayerCharacterControllerVariable : DataVariable<PlayerCharacterController> { }

    [Serializable]
    public class PlayerCharacterControllerReference : DataReference<PlayerCharacterControllerVariable, PlayerCharacterController>
    {
        public PlayerCharacterControllerReference() : base() { }
        public PlayerCharacterControllerReference(PlayerCharacterController value) : base(value) { }
        public PlayerCharacterControllerReference(PlayerCharacterControllerVariable referenceData) : base(referenceData) { }

        public static implicit operator PlayerCharacterControllerReference(PlayerCharacterController value) => new(value);
        public static implicit operator PlayerCharacterControllerReference(PlayerCharacterControllerVariable referenceData) => new(referenceData);
    }
}