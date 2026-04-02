using System;
using UnityEngine;
using UnityEngine.Events;

namespace Potato.Gameplay
{
    public enum WeaponShootType { Manual, Automatic }

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
        [SerializeField] private RangedAttackSystem rats;
        [SerializeField] private WeaponAnimator weaponAnimator;
        [SerializeField] private ProjectileBase projectilePrefab;
        [SerializeField] private GameObject weaponRoot;
        [SerializeField] private Transform weaponMuzzle;

        public UnityAction<Transform> OnShoot;

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
        [SerializeField] private float ammoReloadDelay = 1f;
        [SerializeField] private float shotReloadDelay = 2f;
        [SerializeField] private int maxAmmo = 8;

        private int _currentAmmo = 0;
        private float _lastAmmoTime = Mathf.NegativeInfinity;
        private float _lastShotTime = Mathf.NegativeInfinity;
        private Vector3 _lastMuzzlePosition;

        public GameObject Owner { get; set; }
        public GameObject SourcePrefab { get; set; }
        public int CurrentAmmo => _currentAmmo;
        public int MaxAmmo => maxAmmo;
        public bool IsWeaponActive { get; private set; }
        public Vector3 MuzzleWorldVelocity { get; private set; }
        public float RecoilForce => recoilForce;
        public Vector3 AimOffset => aimOffset;
        public float AimZoomRatio => aimZoomRatio;
        public bool AutomaticReload => automaticReload;
        public bool IsReloading { get; private set; }

        void Awake()
        {
            _currentAmmo = maxAmmo;
            _lastMuzzlePosition = weaponMuzzle.position;
        }

        public void StartReload()
        {
            if (_currentAmmo < maxAmmo)
            {
                weaponAnimator.StartReloadAnimation();
                IsReloading = true;
            }
        }

        void Update()
        {
            UpdateAmmo();

            if (Time.deltaTime > 0)
            {
                MuzzleWorldVelocity = (weaponMuzzle.position - _lastMuzzlePosition) / Time.deltaTime;
                _lastMuzzlePosition = weaponMuzzle.position;
            }
        }

        void UpdateAmmo()
        {
            if (_currentAmmo < maxAmmo && automaticReload && _lastShotTime + shotReloadDelay < Time.time &&  _lastAmmoTime + ammoReloadDelay < Time.time)
            {
                // reloads weapon over time
                _currentAmmo += 1;

                // limits ammo to max value
                _currentAmmo = Math.Clamp(_currentAmmo, 0, maxAmmo);
            }
        }

        public void ShowWeapon(bool show)
        {
            weaponRoot.SetActive(show);
            weaponAnimator.OnShowWeapon();
            IsWeaponActive = show;
        }

        public void UseAmmo(int amount)
        {
            _currentAmmo = Math.Clamp(_currentAmmo - amount, 0, maxAmmo);
        }

        public bool HandleShootInputs(bool inputDown, bool inputHeld)
        {
            switch (shootType)
            {
                case WeaponShootType.Manual:
                    if (inputDown)
                        return TryShoot();
                    return false;

                case WeaponShootType.Automatic:
                    if (inputHeld)
                        return TryShoot();
                    return false;

                default:
                    return false;
            }
        }

        bool TryShoot()
        {
            if (_currentAmmo > 0
                && _lastShotTime + shotCooldown < Time.time)
            {
                HandleShoot();
                UseAmmo(1);

                return true;
            }

            return false;
        }

        void HandleShoot()
        {
            rats.DoProjectileAttack(this, projectilePrefab, weaponMuzzle.position, weaponMuzzle.forward, bulletsPerShot, bulletSpreadAngle);
            weaponAnimator.OnWeaponFired(weaponMuzzle);
            _lastShotTime = Time.time;

            OnShoot?.Invoke(weaponMuzzle);
        }
    }
}