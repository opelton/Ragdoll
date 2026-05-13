using System;
using UnityEngine;

namespace Potato.Gameplay
{

    [Serializable]
    public struct CrosshairData
    {
        public Sprite CrosshairSprite;
        public int CrosshairSize;
    }

    public abstract class WeaponController : MonoBehaviour
    {
        public enum ShootType { Manual, Automatic }

        [SerializeField] protected RangedAttackSystem rats;
        [SerializeField] protected WeaponAnimator weaponAnimator;
        [SerializeField] protected GameObject weaponRoot;
        [SerializeField] protected Transform weaponMeshTransform;

        [Header("WeaponInfo")]
        public string WeaponDisplayName;
        public Sprite WeaponIcon;
        public CrosshairData WeaponCrosshairData;

        [Header("Shoot Parameters")]
        [SerializeField] protected ShootType shootType;
        [SerializeField] protected float shotCooldown = 0.5f;
        [SerializeField] protected float bulletSpreadAngle = 0f;
        [SerializeField] protected int bulletsPerShot = 1;
        [SerializeField] protected float bulletDamage = 1f;
        [SerializeField][Range(0f, 2f)] protected float recoilForce = 1;
        [SerializeField][Range(0f, 1f)] protected float aimZoomRatio = 1f;
        [SerializeField][Range(0f, 1f)] protected float aimSpreadRatio = .5f;

        [Tooltip("Translation to apply to weapon arm when aiming with this weapon")]
        [SerializeField] protected Vector3 aimOffset;

        [Header("Ammo Parameters")]
        [SerializeField] protected float ammoReloadDelay = 1f;
        [SerializeField] protected int maxAmmo = 8;
        [SerializeField] protected int shotsPerReload = 1;

        protected int _currentAmmo = 0;
        protected float _lastShotTime = Mathf.NegativeInfinity;

        public GameObject Owner { get; set; }
        [HideInInspector] public PlayerCamerasController PlayerCams;
        public int CurrentAmmo => _currentAmmo;
        public int MaxAmmo => maxAmmo;
        public Vector3 MuzzleWorldVelocity => weaponAnimator.MuzzleWorldVelocity;
        public float RecoilForce => recoilForce;
        public Vector3 AimOffset => aimOffset;
        public Transform WeaponMeshTransform => weaponMeshTransform;
        public float AimZoomRatio => aimZoomRatio;
        public bool IsReloading { get; protected set; } = false;
        public bool IsAiming { get; protected set; } = false;
        private Vector3[] _hitBuffer;

        protected virtual void Awake()
        {
            _currentAmmo = maxAmmo;
            _hitBuffer = new Vector3[bulletsPerShot];
        }

        public virtual void ShowWeapon(bool show)
        {
            weaponRoot.SetActive(show);

            if(show)
                weaponAnimator.AnimateShowWeapon();
        }

        public abstract bool HandleWeaponInputs(bool fire1Down, bool fire1Held, bool reloadDown, bool isAiming);

        protected virtual void FireWeapon()
        {
            var angle = bulletSpreadAngle;
            if(IsAiming)
                angle *= aimSpreadRatio;

            // when firing multiple shots, guarantee at least one goes toward the crosshair
            if(bulletsPerShot > 1)
            {
                int count = rats.DoBulletAttacks(this, PlayerCams.AimPos, PlayerCams.AimDir, bulletDamage, angle, bulletsPerShot - 1, ref _hitBuffer);
                _hitBuffer[count] = rats.DoBulletAttack(this, PlayerCams.AimPos, PlayerCams.AimDir, bulletDamage, 0f);
            }
            else
                _hitBuffer[0] = rats.DoBulletAttack(this, PlayerCams.AimPos, PlayerCams.AimDir, bulletDamage, angle);

            // Debug.Log($"range {(PlayerCams.AimPos - _hitBuffer[0]).magnitude}");
            weaponAnimator.AnimateWeaponAttack(this, _hitBuffer);
            _lastShotTime = Time.time;
        }
    }
}