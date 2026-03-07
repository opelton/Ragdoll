using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Potato.Gameplay
{
    public enum WeaponShootType { Manual, Automatic, Charge }

    [Serializable]
    public struct CrosshairData
    {
        public Sprite CrosshairSprite;
        public int CrosshairSize;
        public Color CrosshairColor;
    }

    [RequireComponent(typeof(AudioSource))]
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private ProjectileBase projectilePrefab;
        [SerializeField] private GameObject weaponRoot;
        [SerializeField] private Transform weaponMuzzle;

        public UnityAction OnShoot;
        public event Action OnShootProcessed;

        [Header("WeaponInfo")]
        public string WeaponDisplayName;
        public Sprite WeaponIcon;
        public CrosshairData CrosshairData_Default;
        public CrosshairData CrosshairData_TargetingEnemy;

        [Header("Shoot Parameters")]
        [SerializeField] private WeaponShootType shootType;
        [SerializeField] private float shotCooldown = 0.5f;
        [SerializeField] private float bulletSpreadAngle = 0f;
        [SerializeField] private int bulletsPerShot = 1;
        [SerializeField] [Range(0f, 2f)] private float recoilForce = 1;
        [SerializeField] [Range(0f, 1f)] private float aimZoomRatio = 1f;

        [Tooltip("Translation to apply to weapon arm when aiming with this weapon")]
        [SerializeField] private Vector3 aimOffset;

        [Header("Ammo Parameters")]
        [SerializeField] private bool automaticReload = true;
        [SerializeField] private bool usesPhysicalBullets = false;
        [SerializeField] private int magazineSize = 30;
        [SerializeField] private GameObject shellCasing;
        [SerializeField] private Transform ejectionPort;
        [SerializeField] [Range(0.0f, 5.0f)] private float shellCasingEjectionForce = 2.0f;
        [SerializeField] [Range(1, 30)] private int shellPoolSize = 1;
        [SerializeField] private float ammoReloadPerSecond = 1f;
        [SerializeField] private float shotReloadDelay = 2f;
        [SerializeField] private int maxAmmo = 8;

        [Header("Charging parameters (charging weapons only)")]
        [SerializeField] private bool chargeShotAutoRelease;
        [SerializeField] private float maxChargeTime = 2f;
        [SerializeField] private float chargeStartCost = 1f;
        [SerializeField] private float chargeDrainRate = 1f;

        [Header("Audio & Visual")]
        [SerializeField] private Animator weaponAnimator;
        [SerializeField] private GameObject muzzleFlashPrefab;
        [SerializeField] private bool unparentMuzzleFlash;
        [SerializeField] private AudioClip shootSfx;
        [SerializeField] private AudioClip changeWeaponSfx;
        [SerializeField] private bool useContinuousShotSounds = false;
        [SerializeField] private AudioClip continuousShotSfx_Start;
        [SerializeField] private AudioClip continuousShotSfx_Loop;
        [SerializeField] private AudioClip continuousShotSfx_End;


        private AudioSource _continuousShootAudioSource = null;
        private bool _wantsToShoot = false;
        private int _carriedPhysicalBullets;
        private float _currentAmmo;
        private float _lastShotTime = Mathf.NegativeInfinity;
        private Vector3 _lastMuzzlePosition;

        public float LastChargeTriggerTimestamp { get; private set; }
        public GameObject Owner { get; set; }
        public GameObject SourcePrefab { get; set; }
        public bool IsCharging { get; private set; }
        public float CurrentAmmoRatio { get; private set; }
        public bool IsWeaponActive { get; private set; }
        public bool IsCooling { get; private set; }
        public float CurrentCharge { get; private set; }
        public Vector3 MuzzleWorldVelocity { get; private set; }
        public float RecoilForce => recoilForce;
        public Vector3 AimOffset => aimOffset;
        public float AimZoomRatio => aimZoomRatio;
        public bool AutomaticReload => automaticReload;

        public float GetAmmoNeededToShoot() =>
            (shootType != WeaponShootType.Charge ? 1f : Mathf.Max(1f, chargeStartCost)) /
            (maxAmmo * bulletsPerShot);

        public int GetCarriedPhysicalBullets() => _carriedPhysicalBullets;
        public int GetCurrentAmmo() => Mathf.FloorToInt(_currentAmmo);

        AudioSource _shootAudioSource;

        public bool IsReloading { get; private set; }

        const string k_AnimAttackParameter = "Attack";

        private Queue<Rigidbody> _physicalAmmoPool;

        void Awake()
        {
            _currentAmmo = maxAmmo;
            _carriedPhysicalBullets = usesPhysicalBullets ? magazineSize : 0;
            _lastMuzzlePosition = weaponMuzzle.position;

            _shootAudioSource = GetComponent<AudioSource>();

            // if (useContinuousShotSounds)
            // {
            //     _continuousShootAudioSource = gameObject.AddComponent<AudioSource>();
            //     _continuousShootAudioSource.playOnAwake = false;
            //     _continuousShootAudioSource.clip = continuousShotSfx_Loop;
            //     _continuousShootAudioSource.outputAudioMixerGroup =
            //         AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.WeaponShoot);
            //     _continuousShootAudioSource.loop = true;
            // }

            if (usesPhysicalBullets)
            {
                _physicalAmmoPool = new Queue<Rigidbody>(shellPoolSize);

                for (int i = 0; i < shellPoolSize; i++)
                {
                    GameObject shell = Instantiate(shellCasing, transform);
                    shell.SetActive(false);
                    _physicalAmmoPool.Enqueue(shell.GetComponent<Rigidbody>());
                }
            }
        }

        public void AddCarriablePhysicalBullets(int count) => _carriedPhysicalBullets = Mathf.Max(_carriedPhysicalBullets + count, maxAmmo);

        void ShootShell()
        {
            Rigidbody nextShell = _physicalAmmoPool.Dequeue();

            nextShell.transform.position = ejectionPort.transform.position;
            nextShell.transform.rotation = ejectionPort.transform.rotation;
            nextShell.gameObject.SetActive(true);
            nextShell.transform.SetParent(null);
            nextShell.collisionDetectionMode = CollisionDetectionMode.Continuous;
            nextShell.AddForce(nextShell.transform.up * shellCasingEjectionForce, ForceMode.Impulse);

            _physicalAmmoPool.Enqueue(nextShell);
        }

        void PlaySFX(AudioClip sfx)
        {
            // AudioUtility.CreateSFX(sfx, transform.position, AudioUtility.AudioGroups.WeaponShoot, 0.0f);
        }


        void Reload()
        {
            if (_carriedPhysicalBullets > 0)
            {
                _currentAmmo = Mathf.Min(_carriedPhysicalBullets, magazineSize);
            }

            IsReloading = false;
        }

        public void StartReloadAnimation()
        {
            if (_currentAmmo < _carriedPhysicalBullets)
            {
                GetComponent<Animator>().SetTrigger("Reload");
                IsReloading = true;
            }
        }

        void Update()
        {
            UpdateAmmo();
            UpdateCharge();
            UpdateContinuousShootSound();

            if (Time.deltaTime > 0)
            {
                MuzzleWorldVelocity = (weaponMuzzle.position - _lastMuzzlePosition) / Time.deltaTime;
                _lastMuzzlePosition = weaponMuzzle.position;
            }
        }

        void UpdateAmmo()
        {
            if (automaticReload && _lastShotTime + shotReloadDelay < Time.time && _currentAmmo < maxAmmo && !IsCharging)
            {
                // reloads weapon over time
                _currentAmmo += ammoReloadPerSecond * Time.deltaTime;

                // limits ammo to max value
                _currentAmmo = Mathf.Clamp(_currentAmmo, 0, maxAmmo);

                IsCooling = true;
            }
            else
            {
                IsCooling = false;
            }

            if (maxAmmo == Mathf.Infinity)
            {
                CurrentAmmoRatio = 1f;
            }
            else
            {
                CurrentAmmoRatio = _currentAmmo / maxAmmo;
            }
        }

        void UpdateCharge()
        {
            if (IsCharging)
            {
                if (CurrentCharge < 1f)
                {
                    float chargeLeft = 1f - CurrentCharge;

                    // Calculate how much charge ratio to add this frame
                    float chargeAdded = 0f;
                    if (maxChargeTime <= 0f)
                    {
                        chargeAdded = chargeLeft;
                    }
                    else
                    {
                        chargeAdded = (1f / maxChargeTime) * Time.deltaTime;
                    }

                    chargeAdded = Mathf.Clamp(chargeAdded, 0f, chargeLeft);

                    // See if we can actually add this charge
                    float ammoThisChargeWouldRequire = chargeAdded * chargeDrainRate;
                    if (ammoThisChargeWouldRequire <= _currentAmmo)
                    {
                        // Use ammo based on charge added
                        UseAmmo(ammoThisChargeWouldRequire);

                        // set current charge ratio
                        CurrentCharge = Mathf.Clamp01(CurrentCharge + chargeAdded);
                    }
                }
            }
        }

        void UpdateContinuousShootSound()
        {
            if (useContinuousShotSounds)
            {
                if (_wantsToShoot && _currentAmmo >= 1f)
                {
                    if (!_continuousShootAudioSource.isPlaying)
                    {
                        _shootAudioSource.PlayOneShot(shootSfx);
                        _shootAudioSource.PlayOneShot(continuousShotSfx_Start);
                        _continuousShootAudioSource.Play();
                    }
                }
                else if (_continuousShootAudioSource.isPlaying)
                {
                    _shootAudioSource.PlayOneShot(continuousShotSfx_End);
                    _continuousShootAudioSource.Stop();
                }
            }
        }

        public void ShowWeapon(bool show)
        {
            weaponRoot.SetActive(show);

            if (show && changeWeaponSfx)
            {
                _shootAudioSource.PlayOneShot(changeWeaponSfx);
            }

            IsWeaponActive = show;
        }

        public void UseAmmo(float amount)
        {
            _currentAmmo = Mathf.Clamp(_currentAmmo - amount, 0f, maxAmmo);
            _carriedPhysicalBullets -= Mathf.RoundToInt(amount);
            _carriedPhysicalBullets = Mathf.Clamp(_carriedPhysicalBullets, 0, maxAmmo);
            _lastShotTime = Time.time;
        }

        public bool HandleShootInputs(bool inputDown, bool inputHeld, bool inputUp)
        {
            _wantsToShoot = inputDown || inputHeld;
            switch (shootType)
            {
                case WeaponShootType.Manual:
                    if (inputDown)
                    {
                        return TryShoot();
                    }

                    return false;

                case WeaponShootType.Automatic:
                    if (inputHeld)
                    {
                        return TryShoot();
                    }

                    return false;

                case WeaponShootType.Charge:
                    if (inputHeld)
                    {
                        TryBeginCharge();
                    }

                    // Check if we released charge or if the weapon shoot autmatically when it's fully charged
                    if (inputUp || (chargeShotAutoRelease && CurrentCharge >= 1f))
                    {
                        return TryReleaseCharge();
                    }

                    return false;

                default:
                    return false;
            }
        }

        bool TryShoot()
        {
            if (_currentAmmo >= 1f
                && _lastShotTime + shotCooldown < Time.time)
            {
                HandleShoot();
                _currentAmmo -= 1f;

                return true;
            }

            return false;
        }

        bool TryBeginCharge()
        {
            if (!IsCharging
                && _currentAmmo >= chargeStartCost
                && Mathf.FloorToInt((_currentAmmo - chargeStartCost) * bulletsPerShot) > 0
                && _lastShotTime + shotCooldown < Time.time)
            {
                UseAmmo(chargeStartCost);

                LastChargeTriggerTimestamp = Time.time;
                IsCharging = true;

                return true;
            }

            return false;
        }

        bool TryReleaseCharge()
        {
            if (IsCharging)
            {
                HandleShoot();

                CurrentCharge = 0f;
                IsCharging = false;

                return true;
            }

            return false;
        }

        void HandleShoot()
        {
            int bulletsPerShotFinal = shootType == WeaponShootType.Charge
                ? Mathf.CeilToInt(CurrentCharge * bulletsPerShot)
                : bulletsPerShot;

            // spawn all bullets with random direction
            for (int i = 0; i < bulletsPerShotFinal; i++)
            {
                Vector3 shotDirection = GetShotDirectionWithinSpread(weaponMuzzle);
                ProjectileBase newProjectile = Instantiate(projectilePrefab, weaponMuzzle.position,
                    Quaternion.LookRotation(shotDirection));
                newProjectile.Shoot(this);
            }

            // muzzle flash
            if (muzzleFlashPrefab != null)
            {
                GameObject muzzleFlashInstance = Instantiate(muzzleFlashPrefab, weaponMuzzle.position,
                    weaponMuzzle.rotation, weaponMuzzle.transform);
                // Unparent the muzzleFlashInstance
                if (unparentMuzzleFlash)
                {
                    muzzleFlashInstance.transform.SetParent(null);
                }

                Destroy(muzzleFlashInstance, 2f);
            }

            if (usesPhysicalBullets)
            {
                ShootShell();
                _carriedPhysicalBullets--;
            }

            _lastShotTime = Time.time;

            // play shoot SFX
            if (shootSfx && !useContinuousShotSounds)
            {
                _shootAudioSource.PlayOneShot(shootSfx);
            }

            // Trigger attack animation if there is any
            if (weaponAnimator)
            {
                weaponAnimator.SetTrigger(k_AnimAttackParameter);
            }

            OnShoot?.Invoke();
            OnShootProcessed?.Invoke();
        }

        public Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
        {
            float spreadAngleRatio = bulletSpreadAngle / 180f;
            Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,
                spreadAngleRatio);

            return spreadWorldDirection;
        }
    }
}