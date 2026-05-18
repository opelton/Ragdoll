using UnityEngine;
using Potato.Game;

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
        [SerializeField] private GameObject vfx_bulletImpact_wall;
        [SerializeField] private GameObject vfx_bulletImpact_zombie;
        [SerializeField] private float tracerSpeed = 300f;
        [SerializeField] private float pointBlankThreshold = 2f;

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

        public virtual void AnimateWeaponAttack(WeaponController owner, HitInfo[] hitLocations)
        {
            if (muzzleFlashPrefab != null)
            {
                // todo -- could reuse the same emitter instead of instantiating new ones
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

            // fire tracers at the impact points
            if(hitLocations.Length != 0)
            {
                var adjustedMuzzlePosition = junkSpawner.WeaponToGameSpacePosition(weaponMuzzle.position);
                foreach(var hit in hitLocations)
                {
                    if((hit.Point-adjustedMuzzlePosition).sqrMagnitude >= pointBlankThreshold * pointBlankThreshold)
                    {
                        TracerProjectile tracer = Instantiate(tracerPrefab, adjustedMuzzlePosition, Quaternion.identity);
                        tracer.Fire(hit.Point, adjustedMuzzlePosition, tracerSpeed);
                    }
                    
                    if(hit.StruckSurface)
                    {
                        // todo -- more robust system for choosing fx based on surfaces/materials/enemies/etc
                        GameObject impactFx = Instantiate(
                            hit.StruckEnemy ? vfx_bulletImpact_zombie : vfx_bulletImpact_wall,
                            hit.Point, Quaternion.LookRotation(hit.Normal));

                        Destroy(impactFx, 1f);
                    }
                }
            }
        }
    }
}