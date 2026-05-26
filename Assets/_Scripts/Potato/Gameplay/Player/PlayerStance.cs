using UnityEngine;

namespace Potato.Gameplay
{
    public enum WeaponStance { Up, Down, Stowing, Drawing }
    public class PlayerStance : MonoBehaviour
    {
        [HideInInspector] public bool IsAiming = false;
        [HideInInspector] public bool IsWalking = false;
        [HideInInspector] public bool IsCrouched = false;
        [HideInInspector] public bool IsGrounded = false;
        [HideInInspector] public bool WasGrounded = false;
        [HideInInspector] public WeaponStance weaponStance = WeaponStance.Up;

        // [HideInInspector] public Vector2 BulletSpread;
        // [HideInInspector] public float MoveSpeed;
        // [HideInInspector] public float FOV;
        // [HideInInspector] public float WeaponBobTime;
        // [HideInInspector] public float FootstepTime;

        // private bool _aiming;
        // private bool _sprinting;
        // private bool _crouched;
        // private bool _grounded;

        // public bool IsAiming
        // {
        //     get => _aiming;
        //     set => SetAiming(value);
        // }

        // public bool IsSprinting
        // {
        //     get => _sprinting;
        //     set => SetSprinting(value);
        // }

        // public bool IsCrouched
        // {
        //     get => _crouched;
        //     set => SetCrouching(value);
        // }

        // public bool IsGrounded
        // {
        //     get => _grounded;
        //     set => SetGrounded(value);
        // }

        // public void SetAiming(bool isAiming)
        // {
        //     _aiming = isAiming;
        // }

        // public void SetSprinting(bool isSprinting)
        // {
        //     _sprinting = isSprinting;
        // }

        // public void SetCrouching(bool isCrouching)
        // {
        //     _crouched = isCrouching;
        // }

        // public void SetGrounded(bool isGrounded)
        // {
        //     _grounded = isGrounded;
        // }
    }
}