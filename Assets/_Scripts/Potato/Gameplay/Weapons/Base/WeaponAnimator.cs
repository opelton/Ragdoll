using UnityEngine;
using Potato.Game;
using Potato.Core;

namespace Potato.Gameplay
{
    public class WeaponAnimator : MonoBehaviour
    {
        [Header("Base animator params")]
        [SerializeField] private DebrisSystem junkSpawner;
        [SerializeField] protected AudioSystem audioSystem;
        [SerializeField] protected Transform weaponMuzzle;
        [SerializeField] protected GameObject muzzleFlashPrefab;
        [SerializeField] protected bool unparentMuzzleFlash;
        [SerializeField] protected AudioClip shootSfx;
        [SerializeField] protected AudioClip changeWeaponSfx;

        [Header("Shell casing")]
        [SerializeField] private GameObject shellCasing;
        [SerializeField] private Transform shellEjectionPort;
        [SerializeField] private float shellEjectionForce = 2f;
        [SerializeField] private float shellEjectionSpin = 15f;

        [Header("Bullet Tracers")]
        [SerializeField] private TracerProjectile tracerPrefab;
        [SerializeField] private float tracerSpeed = 300f;
        [SerializeField] private float tracerDuration = .5f;
        [SerializeField] private float pointBlankThreshold = .75f;
        [SerializeField] private RangedAttackSystem rats;
        [SerializeField] protected Vector3Reference playerAimPoint;

        public Vector3 MuzzleWorldVelocity { get; private set; }
        public Vector3 EjectorWorldVelocity { get; private set; }

        protected Vector3 _lastMuzzlePosition;
        protected Vector3 _lastEjectorPosition;

        protected virtual void Awake()
        {
            _lastMuzzlePosition = weaponMuzzle.position;
            _lastEjectorPosition = shellEjectionPort.position;
        }

        void FixedUpdate()
        {
            if (Time.deltaTime > 0)
            {
                MuzzleWorldVelocity = (weaponMuzzle.position - _lastMuzzlePosition) / Time.fixedDeltaTime;
                _lastMuzzlePosition = weaponMuzzle.position;

                EjectorWorldVelocity = (shellEjectionPort.position - _lastEjectorPosition) / Time.fixedDeltaTime;
                _lastEjectorPosition = shellEjectionPort.position;
            }
        }

        public virtual void AnimateShowWeapon()
        {
            audioSystem.PlayFirstPersonWeaponAudio(changeWeaponSfx);
        }

        public virtual void AnimateShellEject()
        {
            if(shellCasing == null)
                return;

            junkSpawner.SpawnWeaponSpacePrefabInWorldSpace(shellCasing,
                shellEjectionPort.position,
                shellEjectionPort.rotation,
                (shellEjectionPort.forward * shellEjectionForce) + EjectorWorldVelocity,
                shellEjectionSpin);
        }

        public virtual void AnimateWeaponAttack(WeaponController owner, int bulletCount, float bulletSpreadAngle)
        {
            if (muzzleFlashPrefab != null)
            {
                GameObject muzzleFlashInstance = Instantiate(
                    muzzleFlashPrefab,
                    junkSpawner.WeaponToGameSpacePosition(weaponMuzzle.position),
                    junkSpawner.WeaponToGameSpaceRotation(weaponMuzzle.rotation),
                    weaponMuzzle);

                if (unparentMuzzleFlash)
                    muzzleFlashInstance.transform.SetParent(null);

                // todo -- fx system
                Destroy(muzzleFlashInstance, 2f);
            }

            // play shoot SFX
            if (shootSfx)
                audioSystem.PlayFirstPersonWeaponAudio(shootSfx);

            // fire tracers at the impact point, or straight forward if the impact is too close
            if (tracerPrefab != null)
            {
                var adjustedMuzzlePosition = junkSpawner.WeaponToGameSpacePosition(weaponMuzzle.position);
                var muzzleToTarget = playerAimPoint.Value - adjustedMuzzlePosition;
                //Debug.Log($"shot distance {muzzleToTarget.magnitude}");

                // todo -- below threshold, skip spawning tracers, and just spawn hit fx (decals, sounds, etc)
                // todo -- hit fx (decals, sounds, etc)
                var tracerDirection = muzzleToTarget.sqrMagnitude <= pointBlankThreshold * pointBlankThreshold
                    ? weaponMuzzle.transform.forward
                    : muzzleToTarget.normalized;
                var adjustedOrigin = adjustedMuzzlePosition + Time.deltaTime * MuzzleWorldVelocity;

                rats.FireTracers(owner, tracerPrefab, adjustedOrigin,
                    tracerDirection, bulletCount, bulletSpreadAngle, tracerSpeed, tracerDuration);
            }
        }
    }
}