using UnityEngine;
using Potato.Core;

namespace Potato.Gameplay
{
    public enum WeaponStance { Up, Down, Stowing, Drawing }
    public class PlayerStance : MonoBehaviour
    {
        [HideInInspector] public bool IsAiming = false;
        [HideInInspector] public bool IsWalking = false;
        [HideInInspector] public bool IsCrouched = false;

        [HideInInspector] public Bindable<float> FOVModifier = new();
        [HideInInspector] public Bindable<bool> IsGrounded = new();

        [HideInInspector] public WeaponStance weaponStance = WeaponStance.Up;
    }
}