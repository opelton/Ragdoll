using System;
using UnityEngine;
using Potato.Core;

namespace Potato.Gameplay
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DataVariables/Gameplay/Weapon")]
    public class WeaponVariable : DataVariable<WeaponController> { }

    [Serializable]
    public class WeaponReference : DataReference<WeaponVariable, WeaponController>
    {
        public WeaponReference() : base() { }
        public WeaponReference(WeaponController value) : base(value) { }
        public WeaponReference(WeaponVariable referenceData) : base(referenceData) { }

        public static implicit operator WeaponReference(WeaponController value) => new(value);
        public static implicit operator WeaponReference(WeaponVariable referenceData) => new(referenceData);
    }
}