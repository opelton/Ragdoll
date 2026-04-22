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
            audioSystem.PlayFirstPersonAudio(changeWeaponSfx);
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
                GameObject muzzleFlashInstance = Instantiate(muzzleFlashPrefab, weaponMuzzle.position,
                    weaponMuzzle.rotation, weaponMuzzle);

                if (unparentMuzzleFlash)
                    muzzleFlashInstance.transform.SetParent(null);

                // todo -- fx system
                Destroy(muzzleFlashInstance, 2f);
            }

            // play shoot SFX
            if (shootSfx)
                audioSystem.PlayFirstPersonAudio(shootSfx);

            // fire tracers at the impact point
            if (tracerPrefab != null)
            {
                var tracerDirection = (playerAimPoint.Value - weaponMuzzle.position).normalized;
                var adjustedOrigin = weaponMuzzle.position + Time.deltaTime * MuzzleWorldVelocity;

                rats.FireTracers(owner, tracerPrefab, adjustedOrigin,
                    tracerDirection, bulletCount, bulletSpreadAngle, tracerSpeed, tracerDuration);
            }
        }
    }
}