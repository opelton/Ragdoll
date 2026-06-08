using UnityEngine;
using Potato.Core;
using Potato.Game;
using System;

namespace Potato.Gameplay
{
    [RequireComponent(typeof(PlayerStance))]
    public class FirstPersonAnimationController : MonoBehaviour
    {       
        [Header("Grip poses")]
        [SerializeField] private Transform weaponRoot;
        [SerializeField] private Transform weaponPose_Default;
        [SerializeField] private Transform weaponPose_Aiming;
        [SerializeField] private Transform weaponPose_Down;

        [Header("Grip animation params")]
        [SerializeField] private float weaponBobSharpness = 10f;
        [SerializeField] private Vector2 weaponBob_default = new(0.05f, 0.05f);
        [SerializeField] private Vector2 weaponBob_aiming = new(0.02f, 0.02f);
        [SerializeField] private float recoilSharpness = 50f;
        [SerializeField] private float maxRecoil = 0.5f;
        [SerializeField] private float recoilRecoverySharpness = 10f;
        [SerializeField] private float aimAnimationSpeed = 10f;

        [Header("Sfx")]
        [SerializeField] protected AudioSystem audioSystem;
        [SerializeField] private AudioClip footstepSfx;
        [SerializeField] private AudioClip jumpSfx;
        [SerializeField] private AudioClip landSfx;

        private PlayerStance _stance;
        private Vector3 _weaponRecoilLocalPos;
        private Vector3 _totalRecoil;
        private Vector3 _weaponLocalPos;
        private Quaternion _weaponLocalRotation;
        private Vector3 _weaponBobLocalPos;
        private float _weaponBobMovementScale;

        void Start()
        {
            _stance = GetComponent<PlayerStance>();
            SetWeaponPose_Down();
        }

        // todo -- only update these when they're actually happening, instead of always checking if they are
        void LateUpdate()
        {
            UpdateWeaponRecoil(Time.deltaTime);

            // Set final weapon socket position based on all the combined animation influences
            weaponRoot.SetLocalPositionAndRotation(
                _weaponLocalPos + _weaponBobLocalPos + _weaponRecoilLocalPos,
                _weaponLocalRotation);
        }

        void UpdateWeaponRecoil(float dt)
        {
            // if the accumulated recoil is further away from the current position, make the current position move towards the recoil target
            if (_weaponRecoilLocalPos.z >= _totalRecoil.z * 0.99f)
            {
                _weaponRecoilLocalPos = Vector3.Lerp(_weaponRecoilLocalPos, _totalRecoil,
                    recoilSharpness * dt);
            }
            // otherwise, move recoil position to make it recover towards its resting pose
            else
            {
                _weaponRecoilLocalPos = Vector3.Lerp(_weaponRecoilLocalPos, Vector3.zero,
                    recoilRecoverySharpness * dt);
                _totalRecoil = _weaponRecoilLocalPos;
            }
        }

        public void UpdateWeaponBob(float adjustedMaxSpeed)
        {
            if (Time.deltaTime > 0f)
            {
                // weapon bob magnidue [0,1] from currentSpeed / maxSpeed
                float characterMovementFactor = 0f;
                if (_stance.IsGrounded.Value)
                    characterMovementFactor = Mathf.Clamp01(_stance.Velocity.magnitude / adjustedMaxSpeed);

                // smooth lerp weapon bob magnitude
                var sharpness = weaponBobSharpness * Time.deltaTime;
                _weaponBobMovementScale =
                    Mathf.Lerp(_weaponBobMovementScale, characterMovementFactor, sharpness);

                // StridePhase [0,1] * 2pi = unit circle rotation
                float bobPhase = _stance.StridePhase * 2f * Mathf.PI;
                Vector2 bobAmount = BobMagnitudeFromStance();

                // Calculate vertical and horizontal weapon bob values based on a sine function
                var hBobValue = Mathf.Sin(bobPhase) * bobAmount.x * _weaponBobMovementScale;
                var vBobValue = Mathf.Cos(bobPhase) * bobAmount.y * _weaponBobMovementScale;

                // Apply weapon bob
                _weaponBobLocalPos.x = Mathf.Lerp(_weaponBobLocalPos.x, hBobValue, sharpness);
                _weaponBobLocalPos.y = Mathf.Lerp(_weaponBobLocalPos.y, Mathf.Abs(vBobValue), sharpness);    // abs creates vertical bounce
            }
        }

        Vector2 BobMagnitudeFromStance()
        {
            return _stance.IsAiming || _stance.IsWalking || _stance.IsCrouched
                ? weaponBob_aiming
                : weaponBob_default;
        }

        // Updates weapon position and camera FoV for the aiming transition
        public void LateUpdateWeaponAiming(WeaponController activeWeapon, float dt)
        {
            if (_stance.weaponStance == WeaponStance.Up)
            {
                if (_stance.IsAiming && activeWeapon)
                {
                    _weaponLocalPos = Vector3.Lerp(_weaponLocalPos,
                        weaponPose_Aiming.localPosition + activeWeapon.AimOffset,
                        aimAnimationSpeed * dt);

                    _weaponLocalRotation = Quaternion.Lerp(_weaponLocalRotation,
                        Quaternion.Inverse(activeWeapon.WeaponMeshTransform.localRotation),
                        aimAnimationSpeed * dt);

                    _stance.FOVModifier.Value = Mathf.Lerp(_stance.FOVModifier.Value, activeWeapon.AimZoomRatio, aimAnimationSpeed * dt);
                }
                else
                {
                    _weaponLocalPos = Vector3.Lerp(_weaponLocalPos,
                        weaponPose_Default.localPosition, aimAnimationSpeed * dt);

                    _weaponLocalRotation = Quaternion.Lerp(_weaponLocalRotation,
                        Quaternion.identity,
                        aimAnimationSpeed * dt);

                    _stance.FOVModifier.Value = Mathf.Lerp(_stance.FOVModifier.Value, 1f, aimAnimationSpeed * dt);
                }
            }
        }
        
        // Updates the animated transition of switching weapons
        public void UpdateWeaponSwitchingAnimation(WeaponController activeWeapon, float switchingTimeFactor, bool stowing, bool drawing, float dt)
        {
            if(activeWeapon == null)
                return;

            // Handle moving the weapon socket position for the animated weapon switching
            if (stowing)
            {
                _weaponLocalPos = Vector3.Lerp(weaponPose_Default.localPosition,
                    weaponPose_Down.localPosition, switchingTimeFactor);

                _weaponLocalRotation = Quaternion.Lerp(_weaponLocalRotation,
                        weaponPose_Down.transform.localRotation,
                        aimAnimationSpeed * dt);
            }
            else if (drawing)
            {
                _weaponLocalPos = Vector3.Lerp(weaponPose_Down.localPosition,
                    weaponPose_Default.localPosition, switchingTimeFactor);

                _weaponLocalRotation = Quaternion.Lerp(_weaponLocalRotation,
                        weaponPose_Default.transform.localRotation,
                        aimAnimationSpeed * dt);
            }
        }

        void SetWeaponPose_Down() => _weaponLocalPos = weaponPose_Down.localPosition;

        // todo -- better animation controls (state-based?) recoil time + position
        public void AnimateRecoil(float recoilForce)
        {
            _totalRecoil += Vector3.back * recoilForce;
            _totalRecoil = Vector3.ClampMagnitude(_totalRecoil, maxRecoil);
        }

        public void PlayFootstepSfx() => audioSystem.PlayFirstPersonAudio(footstepSfx);
        public void PlaySfx_Jump() => audioSystem.PlayFirstPersonAudio(jumpSfx);
        public void PlaySfx_Land() => audioSystem.PlayFirstPersonAudio(landSfx);
    }
}