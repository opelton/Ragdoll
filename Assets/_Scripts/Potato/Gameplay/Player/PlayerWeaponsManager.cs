using System.Collections.Generic;
using UnityEngine;
using Potato.Core;
using Potato.Game;

namespace Potato.Gameplay
{
    public class WeaponAttackInfo
    {
        public Vector3 shotOrigin;
        public Vector3 shotDirection;
        public float recoilForce;
    }

    [RequireComponent(typeof(FirstPersonAnimationController))]
    public class PlayerWeaponsManager : MonoBehaviour
    {
        public enum WeaponStance { Up, Down, Stowing, Drawing }

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
        [SerializeField] private WeaponAttackEvent onShootEvent;
        [SerializeField] private WeaponReference activeWeaponRef;
        [SerializeField] private IntReference playerCurrentAmmo;        

        public bool IsAiming { get; private set; }
        public int ActiveWeaponIndex { get; private set; }
        public float WeaponSwitchTimingFactor { get; private set; } = 0f;
        public Vector3 AimPosition => playerCams.AimPos;
        public Vector3 AimDirection => playerCams.AimDir;
        public WeaponStance Stance => _weaponStance;

        // ---
        private WeaponController[] _weaponSlots = new WeaponController[9]; // 9 available weapon slots
        private float _weaponSwitchStartTime;
        private WeaponStance _weaponStance;
        private int _nextWeaponIndex;

        // first person animator should have the weapon audio source, not each weapon prefab
        void Start()
        {
            ActiveWeaponIndex = -1;
            _weaponStance = WeaponStance.Down;

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
                if (_weaponStance == WeaponStance.Up)
                {
                    // handle aiming down sights
                    IsAiming = fire2Input.ButtonDown && !activeWeapon.IsReloading;

                    // handle shooting
                    bool hasFired = activeWeapon.HandleWeaponInputs(
                        fire1Input.ButtonPressed,
                        fire1Input.ButtonDown,
                        reloadInput.ButtonPressed);

                    if (hasFired)
                        onShootEvent.Invoke(GetWeaponAttackInfo(activeWeapon), this);
                }

                if(playerCurrentAmmo.Value != activeWeapon.CurrentAmmo)
                    playerCurrentAmmo.Value = activeWeapon.CurrentAmmo;
            }
        }

        // Update various animated features in LateUpdate because it needs to override the animated arm position
        void LateUpdate()
        {
            UpdateWeaponSwitching();
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
                    _weaponStance = WeaponStance.Drawing;
                    ActiveWeaponIndex = _nextWeaponIndex;

                    WeaponController newWeapon = GetWeaponAtSlotIndex(_nextWeaponIndex);
                    OnWeaponSwitched(newWeapon);
                }
                // otherwise, remember we are putting down our current weapon for switching to the next one
                else
                {
                    _weaponStance = WeaponStance.Stowing;
                }
            }
        }

        // Updates the animated transition of switching weapons
        void UpdateWeaponSwitching()
        {
            // Calculate the time ratio (0 to 1) since weapon switch was triggered
            WeaponSwitchTimingFactor = weaponSwitchDelay == 0f ? 1f : Mathf.Clamp01((Time.time - _weaponSwitchStartTime) / weaponSwitchDelay);

            // Handle transiting to new switch state
            if (WeaponSwitchTimingFactor >= 1f)
            {
                if (_weaponStance == WeaponStance.Stowing)
                {
                    // Deactivate old weapon
                    WeaponController oldWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
                    if (oldWeapon != null)
                        oldWeapon.ShowWeapon(false);

                    ActiveWeaponIndex = _nextWeaponIndex;
                    WeaponSwitchTimingFactor = 0f;

                    // Activate new weapon
                    WeaponController newWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
                    OnWeaponSwitched(newWeapon);

                    if (newWeapon)
                    {
                        _weaponSwitchStartTime = Time.time;
                        _weaponStance = WeaponStance.Drawing;
                    }
                    else
                        _weaponStance = WeaponStance.Down;
                }
                else if (_weaponStance == WeaponStance.Drawing)
                    _weaponStance = WeaponStance.Up;
            }
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
                    weaponInstance.Owner = gameObject;
                    weaponInstance.PlayerCams = playerCams;
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
        
        WeaponAttackInfo GetWeaponAttackInfo(WeaponController activeWeapon)
        {
            return new()
            {
                shotOrigin = AimPosition,
                shotDirection = AimDirection,
                recoilForce = activeWeapon.RecoilForce
            };
        }
    }
}