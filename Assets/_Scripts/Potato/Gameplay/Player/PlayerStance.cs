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
        [HideInInspector] public Vector3 Velocity = Vector3.zero;
        [HideInInspector] public float StridePhase = 0f;

        [HideInInspector] public Bindable<float> FOVModifier = new();
        [HideInInspector] public Bindable<bool> IsGrounded = new();
        [HideInInspector] public Bindable<WeaponStance> WeaponPose = new(WeaponStance.Up);
    }
}