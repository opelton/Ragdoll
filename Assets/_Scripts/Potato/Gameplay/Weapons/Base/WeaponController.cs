using System;
using Potato.Core;
using Potato.Game;
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
        [SerializeField] protected Vector3Reference playerAimPoint;
        [SerializeField] protected ProjectileBase projectilePrefab;
        [SerializeField] protected GameObject weaponRoot;
        [SerializeField] protected Transform weaponMeshTransform;
        [SerializeField] protected Transform weaponMuzzle;

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

        protected virtual void Awake()
        {
            _currentAmmo = maxAmmo;
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
                rats.DoHitscanAttack(this, PlayerCams.AimPos, PlayerCams.AimDir, bulletDamage, 1, 0f);
                rats.DoHitscanAttack(this, PlayerCams.AimPos, PlayerCams.AimDir, bulletDamage, bulletsPerShot - 1, angle);
            }
            else
                rats.DoHitscanAttack(this, PlayerCams.AimPos, PlayerCams.AimDir, bulletDamage, bulletsPerShot, angle);

            weaponAnimator.AnimateWeaponAttack(this, bulletsPerShot, angle);
            _lastShotTime = Time.time;
        }
    }
}