using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Potato.Core;
using Potato.Game;

namespace Potato.Gameplay
{
    [RequireComponent(typeof(FirstPersonAnimationController))]
    public class PlayerWeaponsManager : MonoBehaviour
    {
        public enum WeaponReadyState { Up, Down, PutDownPrevious, PutUpNew }

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
        [SerializeField] private BoolReference isAimingAtEnemy;
        [SerializeField] private IntReference playerCurrentAmmo;        
        [SerializeField] private IntReference playerMaxAmmo;
        public UnityAction<WeaponController> OnSwitchedToWeapon;

        public bool IsAiming { get; private set; }
        public int ActiveWeaponIndex { get; private set; }
        public Vector3 AimPosition => playerCams.AimPos;
        public Vector3 AimDirection => playerCams.AimDir;
        public WeaponReadyState ReadyState => _weaponReadiness;

        // ---
        private FirstPersonAnimationController _playerAnim;
        private WeaponController[] _weaponSlots = new WeaponController[9]; // 9 available weapon slots
        private float _weaponSwitchStartTime;
        private WeaponReadyState _weaponReadiness;
        private int _nextWeaponIndex;

        void Start()
        {
            ActiveWeaponIndex = -1;
            _weaponReadiness = WeaponReadyState.Down;
            _playerAnim = GetComponent<FirstPersonAnimationController>();

            // Add starting weapons
            foreach (var weapon in StartingWeapons)
                AddWeapon(weapon);

            SwitchWeapon(true);
        }

        void Update()
        {
            WeaponController activeWeapon = GetActiveWeapon();

            if (activeWeapon != null)
            {
                if(activeWeapon.IsReloading)
                    return;

                if (_weaponReadiness == WeaponReadyState.Up)
                {
                    if (!activeWeapon.AutomaticReload && reloadInput.ButtonPressed && activeWeapon.CurrentAmmo < activeWeapon.MaxAmmo)
                    {
                        IsAiming = false;
                        activeWeapon.StartReloadAnimation();
                        return;
                    }

                    // handle aiming down sights
                    IsAiming = fire2Input.ButtonDown;

                    // handle shooting
                    bool hasFired = activeWeapon.HandleShootInputs(
                        fire1Input.ButtonPressed,
                        fire1Input.ButtonDown);

                    // Handle accumulating recoil
                    if (hasFired)
                        _playerAnim.OnWeaponFired(activeWeapon.RecoilForce);
                }

                if(playerCurrentAmmo.Value != activeWeapon.CurrentAmmo)
                    playerCurrentAmmo.Value = activeWeapon.CurrentAmmo;
            }

            // weapon switch handling
            if ((_weaponReadiness == WeaponReadyState.Up || _weaponReadiness == WeaponReadyState.Down)
                && !IsAiming && activeWeapon == null)
            {
                if(weaponSwitchInput.ButtonPressed)
                    SwitchWeapon(true);
            }

            // Pointing at enemy handling
            var targetingHostile = activeWeapon != null && rats.IsTargetingEnemy(gameObject, playerCams.AimPos, playerCams.AimDir);
            
            // avoid firing an onChanged event unless it changed
            if(isAimingAtEnemy.Value != targetingHostile)
                isAimingAtEnemy.Value = targetingHostile;
        }


        // Update various animated features in LateUpdate because it needs to override the animated arm position
        void LateUpdate()
        {
            UpdateWeaponSwitching();
        }

        // Iterate on all weapon slots to find the next valid weapon to switch to
        public void SwitchWeapon(bool ascendingOrder)
        {
            int newWeaponIndex = -1;
            int closestSlotDistance = _weaponSlots.Length;
            for (int i = 0; i < _weaponSlots.Length; i++)
            {
                // If the weapon at this slot is valid, calculate its "distance" from the active slot index (either in ascending or descending order)
                // and select it if it's the closest distance yet
                if (i != ActiveWeaponIndex && GetWeaponAtSlotIndex(i) != null)
                {
                    int distanceToActiveIndex = GetDistanceBetweenWeaponSlots(ActiveWeaponIndex, i, ascendingOrder);

                    if (distanceToActiveIndex < closestSlotDistance)
                    {
                        closestSlotDistance = distanceToActiveIndex;
                        newWeaponIndex = i;
                    }
                }
            }

            // Handle switching to the new weapon index
            SwitchToWeaponIndex(newWeaponIndex);
        }

        // Switches to the given weapon index in weapon slots if the new index is a valid weapon that is different from our current one
        public void SwitchToWeaponIndex(int newWeaponIndex, bool force = false)
        {
            if (force || (newWeaponIndex != ActiveWeaponIndex && newWeaponIndex >= 0))
            {
                // Store data related to weapon switching animation
                _nextWeaponIndex = newWeaponIndex;
                _weaponSwitchStartTime = Time.time;

                // Handle case of switching to a valid weapon for the first time (simply put it up without putting anything down first)
                if (GetActiveWeapon() == null)
                {
                    _playerAnim.SetWeaponPose_Down();
                    _weaponReadiness = WeaponReadyState.PutUpNew;
                    ActiveWeaponIndex = _nextWeaponIndex;

                    WeaponController newWeapon = GetWeaponAtSlotIndex(_nextWeaponIndex);
                    OnWeaponSwitched(newWeapon);
                }
                // otherwise, remember we are putting down our current weapon for switching to the next one
                else
                {
                    _weaponReadiness = WeaponReadyState.PutDownPrevious;
                }
            }
        }

        // Updates the animated transition of switching weapons
        void UpdateWeaponSwitching()
        {
            // Calculate the time ratio (0 to 1) since weapon switch was triggered
            float switchingTimeFactor = weaponSwitchDelay == 0f ? 1f : Mathf.Clamp01((Time.time - _weaponSwitchStartTime) / weaponSwitchDelay);

            // Handle transiting to new switch state
            if (switchingTimeFactor >= 1f)
            {
                if (_weaponReadiness == WeaponReadyState.PutDownPrevious)
                {
                    // Deactivate old weapon
                    WeaponController oldWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
                    if (oldWeapon != null)
                        oldWeapon.ShowWeapon(false);

                    ActiveWeaponIndex = _nextWeaponIndex;
                    switchingTimeFactor = 0f;

                    // Activate new weapon
                    WeaponController newWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
                    OnWeaponSwitched(newWeapon);

                    if (newWeapon)
                    {
                        _weaponSwitchStartTime = Time.time;
                        _weaponReadiness = WeaponReadyState.PutUpNew;
                    }
                    else
                        _weaponReadiness = WeaponReadyState.Down;
                }
                else if (_weaponReadiness == WeaponReadyState.PutUpNew)
                    _weaponReadiness = WeaponReadyState.Up;
            }

            _playerAnim.UpdateWeaponSwitchingAnimation(switchingTimeFactor);
        }

        // Adds a weapon to our inventory
        public bool AddWeapon(WeaponController weaponPrefab)
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
                    weaponInstance.SourcePrefab = weaponPrefab.gameObject;
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
                SwitchWeapon(true);

            return false;
        }


        public WeaponController GetActiveWeapon() => GetWeaponAtSlotIndex(ActiveWeaponIndex);

        public WeaponController GetWeaponAtSlotIndex(int index)
        {
            // find the active weapon in our weapon slots based on our active weapon index
            if (index >= 0 && index < _weaponSlots.Length)
                return _weaponSlots[index];

            // if we didn't find a valid active weapon in our weapon slots, return null
            return null;
        }

        // Calculates the "distance" between two weapon slot indexes
        // For example: if we had 5 weapon slots, the distance between slots #2 and #4 would be 2 in ascending order, and 3 in descending order
        int GetDistanceBetweenWeaponSlots(int fromSlotIndex, int toSlotIndex, bool ascendingOrder)
        {
            int distanceBetweenSlots = 0;

            if (ascendingOrder)
                distanceBetweenSlots = toSlotIndex - fromSlotIndex;
            else
                distanceBetweenSlots = -1 * (toSlotIndex - fromSlotIndex);

            if (distanceBetweenSlots < 0)
                distanceBetweenSlots = _weaponSlots.Length + distanceBetweenSlots;

            return distanceBetweenSlots;
        }

        public void OnWeaponSwitched(WeaponController newGun)
        {
            if (newGun != null)
                newGun.ShowWeapon(true);
            
            playerMaxAmmo.Value = newGun.MaxAmmo;
        }        
    }
}