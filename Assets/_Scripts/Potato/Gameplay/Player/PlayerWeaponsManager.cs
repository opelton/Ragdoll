using System.Collections.Generic;
using UnityEngine;
using Potato.Core;
using Potato.Game;

namespace Potato.Gameplay
{
    [RequireComponent(typeof(FirstPersonAnimationController), typeof(PlayerStance))]
    public class PlayerWeaponsManager : MonoBehaviour
    {
        [SerializeField] private RangedAttackSystem rats;
        [SerializeField] private Transform weaponRoot;

        [Header("Loadout")]
        [SerializeField] private List<WeaponController> StartingWeapons = new();

        [Header("Camera")]
        [SerializeField][LayerIndex] private int fpsWeaponsLayer;
        [SerializeField] private PlayerCamerasController playerCams;

        [Header("Input Data")]
        [SerializeField] private InputButton fire1Input;
        [SerializeField] private InputButton fire2Input;
        [SerializeField] private InputButton reloadInput;
        [SerializeField] private InputButton weaponSwitchInput;

        [Header("Settings")]
        [SerializeField] private LayerMask hitboxLayers;
        [SerializeField] private float weaponSwitchDelay = 1f;

        [Header("Out Data")]
        [SerializeField] private WeaponReference activeWeaponRef;
        [SerializeField] private IntReference playerCurrentAmmo;        

        public int ActiveWeaponIndex { get; private set; }
        public float WeaponSwitchTimingFactor { get; private set; } = 0f;
        public PlayerCamerasController AimCams => playerCams;

        // ---
        private WeaponController[] _weaponSlots = new WeaponController[9]; // 9 available weapon slots
        private FirstPersonAnimationController _animationController;
        private PlayerStance _stance;
        private float _weaponSwitchStartTime;
        private int _nextWeaponIndex;

        // first person animator should have the weapon audio source, not each weapon prefab
        void Start()
        {
            _animationController = GetComponent<FirstPersonAnimationController>();
            _stance = GetComponent<PlayerStance>();

            ActiveWeaponIndex = -1;
            _stance.weaponStance = WeaponStance.Down;

            // Add starting weapons
            foreach (var weapon in StartingWeapons)
                AddWeapon(weapon);

            SwapWeapon();
        }

        // todo -- should aiming actually prevent reloading? Should it be a gun stat?
        void Update()
        {
            WeaponController activeWeapon = GetActiveWeapon();

            if (activeWeapon != null)
            {
                if (_stance.weaponStance == WeaponStance.Up)
                {
                    // handle aiming down sights
                    _stance.IsAiming = fire2Input.ButtonDown;

                    // handle shooting
                    bool hasFired = activeWeapon.HandleWeaponInputs(
                        fire1Input.ButtonPressed,
                        fire1Input.ButtonDown,
                        reloadInput.ButtonPressed,
                        _stance.IsAiming);

                    if (hasFired)
                        _animationController.AnimateRecoil(activeWeapon.RecoilForce);
                }

                if(playerCurrentAmmo.Value != activeWeapon.CurrentAmmo)
                    playerCurrentAmmo.Value = activeWeapon.CurrentAmmo;
            }
        }

        // Update various animated features in LateUpdate because it needs to override the animated arm position
        void LateUpdate()
        {
            var dt = Time.deltaTime;
            UpdateWeaponSwitching(dt);
            _animationController.LateUpdateWeaponAiming(GetActiveWeapon(), dt);
        }

        // Switches to the given weapon index in weapon slots if the new index is a valid weapon that is different from our current one
        void SwitchToWeaponIndex(int newWeaponIndex, bool force = false)
        {
            if (force || (newWeaponIndex != ActiveWeaponIndex && newWeaponIndex >= 0))
            {
                // Store data related to weapon switching animation
                _nextWeaponIndex = newWeaponIndex;
                _weaponSwitchStartTime = Time.time;

                // Handle case of switching to a valid weapon for the first time (simply put it up without putting anything down first)
                if (GetActiveWeapon() == null)
                {
                    _stance.weaponStance = WeaponStance.Drawing;
                    ActiveWeaponIndex = _nextWeaponIndex;

                    WeaponController newWeapon = GetWeaponAtSlotIndex(_nextWeaponIndex);
                    OnWeaponSwitched(newWeapon);
                }
                // otherwise, remember we are putting down our current weapon for switching to the next one
                else
                {
                    _stance.weaponStance = WeaponStance.Stowing;
                }
            }
        }

        // Updates the animated transition of switching weapons
        void UpdateWeaponSwitching(float dt)
        {
            // Calculate the time ratio (0 to 1) since weapon switch was triggered
            WeaponSwitchTimingFactor = weaponSwitchDelay == 0f ? 1f : Mathf.Clamp01((Time.time - _weaponSwitchStartTime) / weaponSwitchDelay);
            WeaponController activeWeapon = GetActiveWeapon();

            // Handle transiting to new switch state
            if (WeaponSwitchTimingFactor >= 1f)
            {
                if (_stance.weaponStance == WeaponStance.Stowing)
                {
                    // Deactivate old weapon
                    if (activeWeapon != null)
                        activeWeapon.ShowWeapon(false);

                    ActiveWeaponIndex = _nextWeaponIndex;
                    WeaponSwitchTimingFactor = 0f;

                    // Activate new weapon
                    activeWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
                    OnWeaponSwitched(activeWeapon);

                    if (activeWeapon)
                    {
                        _weaponSwitchStartTime = Time.time;
                        _stance.weaponStance = WeaponStance.Drawing;
                    }
                    else
                        _stance.weaponStance = WeaponStance.Down;
                }
                else if (_stance.weaponStance == WeaponStance.Drawing)
                    _stance.weaponStance = WeaponStance.Up;
            }

            _animationController.UpdateWeaponSwitchingAnimation(
                activeWeapon,
                WeaponSwitchTimingFactor,
                _stance.weaponStance == WeaponStance.Stowing,
                _stance.weaponStance == WeaponStance.Drawing,
                dt);
        }

        // Adds a weapon to our inventory
        bool AddWeapon(WeaponController weaponPrefab)
        {
            // search our weapon slots for the first free one, assign the weapon to it, and return true if we found one. Return false otherwise
            for (int i = 0; i < _weaponSlots.Length; i++)
            {
                // only add the weapon if the slot is free
                if (_weaponSlots[i] == null)
                {
                    // spawn the weapon prefab as child of the weapon socket
                    WeaponController weaponInstance = Instantiate(weaponPrefab, weaponRoot);
                    weaponInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                    // Set owner to this gameObject so the weapon can alter projectile/damage logic accordingly
                    weaponInstance.Owner = this;
                    weaponInstance.ShowWeapon(false);

                    // Assign the first person layer to the weapon
                    foreach (Transform t in weaponInstance.gameObject.GetComponentsInChildren<Transform>(true))
                        t.gameObject.layer = fpsWeaponsLayer;

                    _weaponSlots[i] = weaponInstance;

                    return true;
                }
            }

            // Handle auto-switching to weapon if no weapons currently
            if (GetActiveWeapon() == null)
                SwapWeapon();

            return false;
        }


        public WeaponController GetActiveWeapon() => GetWeaponAtSlotIndex(ActiveWeaponIndex);

        WeaponController GetWeaponAtSlotIndex(int index)
        {
            // find the active weapon in our weapon slots based on our active weapon index
            if (index >= 0 && index < _weaponSlots.Length)
                return _weaponSlots[index];

            // if we didn't find a valid active weapon in our weapon slots, return null
            return null;
        }

        void OnWeaponSwitched(WeaponController newGun)
        {
            if (newGun != null)
                newGun.ShowWeapon(true);
            
            activeWeaponRef.Value = newGun;
        }

        public void SwapWeapon()
        {
            var nextIndex = ActiveWeaponIndex + 1;
            if(nextIndex >= _weaponSlots.Length || _weaponSlots[nextIndex] == null)
                nextIndex = 0;

            SwitchToWeaponIndex(nextIndex);
        }
    }
}