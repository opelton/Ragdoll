using UnityEngine;
using Potato.Core;
using Potato.Game;

namespace Potato.Gameplay
{
    public class FirstPersonAnimationController : MonoBehaviour
    {       
        [Header("Grip poses")]
        [SerializeField] private Transform weaponRoot;
        [SerializeField] private Transform weaponPose_Default;
        [SerializeField] private Transform weaponPose_Aiming;
        [SerializeField] private Transform weaponPose_Down;

        [Header("Grip animation params")]
        [SerializeField] private float weaponBobFrequency = 10f;
        [SerializeField] private float weaponBobSharpness = 10f;
        [SerializeField] private float weaponBob_default = 0.05f;
        [SerializeField] private float weaponBob_aiming = 0.02f;
        [SerializeField] private float recoilSharpness = 50f;
        [SerializeField] private float maxRecoil = 0.5f;
        [SerializeField] private float recoilRecoverySharpness = 10f;
        [SerializeField] private float aimAnimationSpeed = 10f;
        [SerializeField] private FloatReference playerStanceFovModifier;

        [Header("Sfx")]
        [SerializeField] protected AudioSystem audioSystem;
        [SerializeField] private AudioClip footstepSfx;
        [SerializeField] private AudioClip jumpSfx;
        [SerializeField] private AudioClip landSfx;

        [Header("Sfx params")]
        [SerializeField] private float footstepFrequency = .3f;
        [SerializeField] private float footstepFrequencySprinting = .2f;

        private Vector3 _weaponRecoilLocalPos;
        private Vector3 _totalRecoil;
        private Vector3 _weaponLocalPos;
        private Quaternion _weaponLocalRotation;
        private Vector3 _weaponBobLocalPos;
        private Vector3 _lastPlayerPos;
        private float _weaponBobFactor;
        private float _footstepDistanceCounter = 0f;

        void Start()
        {
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

        public void LateUpdateWeaponBob(Vector3 playerPos, bool isGrounded, bool isAiming, float maxGroundSpeed, float sprintModifier)
        {
            if (Time.deltaTime > 0f)
            {
                Vector3 playerCharacterVelocity =
                    (playerPos - _lastPlayerPos) / Time.deltaTime;

                // calculate a smoothed weapon bob amount based on how close to our max grounded movement velocity we are
                float characterMovementFactor = 0f;
                if (isGrounded)
                {
                    characterMovementFactor = Mathf.Clamp01(
                        playerCharacterVelocity.magnitude / (maxGroundSpeed * sprintModifier));
                }

                _weaponBobFactor =
                    Mathf.Lerp(_weaponBobFactor, characterMovementFactor, weaponBobSharpness * Time.deltaTime);

                // Calculate vertical and horizontal weapon bob values based on a sine function
                float bobAmount = isAiming ? weaponBob_aiming : weaponBob_default;
                float frequency = weaponBobFrequency;
                float hBobValue = Mathf.Sin(Time.time * frequency) * bobAmount * _weaponBobFactor;
                float vBobValue = ((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * bobAmount *
                                  _weaponBobFactor;

                // Apply weapon bob
                _weaponBobLocalPos.x = hBobValue;
                _weaponBobLocalPos.y = Mathf.Abs(vBobValue);

                _lastPlayerPos = playerPos;
            }
        }

        // Updates weapon position and camera FoV for the aiming transition
        public void LateUpdateWeaponAiming(WeaponController activeWeapon, bool stanceUp, bool isAiming, float dt)
        {
            if (stanceUp)
            {
                if (isAiming && activeWeapon)
                {
                    _weaponLocalPos = Vector3.Lerp(_weaponLocalPos,
                        weaponPose_Aiming.localPosition + activeWeapon.AimOffset,
                        aimAnimationSpeed * dt);

                    _weaponLocalRotation = Quaternion.Lerp(_weaponLocalRotation,
                        Quaternion.Inverse(activeWeapon.WeaponMeshTransform.localRotation),
                        aimAnimationSpeed * dt);

                    playerStanceFovModifier.Value = Mathf.Lerp(playerStanceFovModifier.Value, activeWeapon.AimZoomRatio, aimAnimationSpeed * dt);
                }
                else
                {
                    _weaponLocalPos = Vector3.Lerp(_weaponLocalPos,
                        weaponPose_Default.localPosition, aimAnimationSpeed * dt);

                    _weaponLocalRotation = Quaternion.Lerp(_weaponLocalRotation,
                        Quaternion.identity,
                        aimAnimationSpeed * dt);

                    playerStanceFovModifier.Value = Mathf.Lerp(playerStanceFovModifier.Value, 1f, aimAnimationSpeed * dt);
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

        public void UpdateFootstepSfx(float moveDistance, bool isSprinting)
        {
            // footsteps sound
            float chosenFootstepSfxFrequency = isSprinting ? footstepFrequencySprinting : footstepFrequency;
            if (_footstepDistanceCounter >= 1f/ chosenFootstepSfxFrequency)
            {
                _footstepDistanceCounter = 0;
                audioSystem.PlayFirstPersonAudio(footstepSfx);
            }

            // keep track of distance traveled for footsteps sound
            _footstepDistanceCounter += moveDistance;
        }

        public void PlaySfx_Jump() => audioSystem.PlayFirstPersonAudio(jumpSfx);
        public void PlaySfx_Land() => audioSystem.PlayFirstPersonAudio(landSfx);
    }
}