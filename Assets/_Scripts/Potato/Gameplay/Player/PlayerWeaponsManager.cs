using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Potato.Core;
using Potato.Game;

namespace Potato.Gameplay
{
    [RequireComponent(typeof(PlayerCharacterController))]
    public class PlayerWeaponsManager : MonoBehaviour
    {
        public enum WeaponReadyState { Up, Down, PutDownPrevious, PutUpNew }

        [Header("Loadout")]
        [SerializeField] private List<WeaponController> StartingWeapons = new();

        [Header("Camera")]
        [SerializeField][LayerIndex] private int fpsWeaponsLayer;
        // todo -- camera system
        [SerializeField] private Camera mainCamera;
        public Camera weaponCamera;

        [Header("Input Data")]
        [SerializeField] private InputButton fire1Input;
        [SerializeField] private InputButton fire2Input;
        [SerializeField] private InputButton reloadInput;
        [SerializeField] private InputButton weaponSwitchInput;

        [Header("Settings")]
        [SerializeField] private IntReference defaultFovRef;
        [SerializeField] private float weaponFovMultiplier = 1f;
        [SerializeField] private float WeaponSwitchDelay = 1f;

        [Header("Out Data")]
        [SerializeField] private WeaponReference activeWeaponRef;
        public UnityAction<WeaponController> OnSwitchedToWeapon;
        // public UnityAction<WeaponController, int> OnAddedWeapon;
        // public UnityAction<WeaponController, int> OnRemovedWeapon;

        [Header("Weapon Animation")]
        [SerializeField] private Transform weaponRoot;
        [SerializeField] private Transform weaponPose_Default;
        [SerializeField] private Transform weaponPose_Aiming;
        [SerializeField] private Transform weaponPose_Down;
        [SerializeField] private float weaponBobFrequency = 10f;
        [SerializeField] private float weaponBobSharpness = 10f;
        [SerializeField] private float weaponBob_default = 0.05f;
        [SerializeField] private float weaponBob_aiming = 0.02f;
        [SerializeField] private float recoilSharpness = 50f;
        [SerializeField] private float maxRecoil = 0.5f;
        [SerializeField] private float recoilRecoverySharpness = 10f;
        [SerializeField] private float aimAnimationSpeed = 10f;

        public bool IsAiming { get; private set; }
        public bool IsPointingAtEnemy { get; private set; }
        public int ActiveWeaponIndex { get; private set; }

        // ---
        private PlayerCharacterController _player;
        private WeaponController[] _weaponSlots = new WeaponController[9]; // 9 available weapon slots
        private float _weaponBobFactor;
        private Vector3 _lastPlayerPos;
        private Vector3 _weaponLocalPos;
        private Vector3 _weaponBobLocalPos;
        private Vector3 _weaponRecoilLocalPos;
        private Vector3 _totalRecoil;
        private float _weaponSwitchStartTime;
        private WeaponReadyState _weaponReadiness;
        private int _nextWeaponIndex;

        void Start()
        {
            ActiveWeaponIndex = -1;
            _weaponReadiness = WeaponReadyState.Down;

            _player = GetComponent<PlayerCharacterController>();

            SetFov(defaultFovRef.Value);

            OnSwitchedToWeapon += OnWeaponSwitched;

            // Add starting weapons
            foreach (var weapon in StartingWeapons)
                AddWeapon(weapon);

            SwitchWeapon(true);
        }

        void Update()
        {
            // shoot handling
            WeaponController activeWeapon = GetActiveWeapon();

            if (activeWeapon != null && activeWeapon.IsReloading)
                return;

            if (activeWeapon != null && _weaponReadiness == WeaponReadyState.Up)
            {
                if (!activeWeapon.AutomaticReload && reloadInput.ButtonPressed && activeWeapon.CurrentAmmoRatio < 1.0f)
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
                    fire1Input.ButtonDown,
                    fire1Input.ButtonReleased);

                // Handle accumulating recoil
                if (hasFired)
                {
                    _totalRecoil += Vector3.back * activeWeapon.RecoilForce;
                    _totalRecoil = Vector3.ClampMagnitude(_totalRecoil, maxRecoil);
                }
            }

            // weapon switch handling
            if (!IsAiming &&
                (activeWeapon == null || !activeWeapon.IsCharging) &&
                (_weaponReadiness == WeaponReadyState.Up || _weaponReadiness == WeaponReadyState.Down))
            {
                if(weaponSwitchInput.ButtonPressed)
                    SwitchWeapon(true);
            }

            // Pointing at enemy handling
            IsPointingAtEnemy = false;
            if (activeWeapon)
            {
                var hits = Physics.RaycastAll(weaponCamera.transform.position, weaponCamera.transform.forward, 1000, -1, QueryTriggerInteraction.Ignore);

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject == gameObject) continue;

                    IsPointingAtEnemy = true;
                    break;

                    // todo -- enemy/team/etc component
                    // if (hit.collider.GetComponentInParent<Health>() != null)
                    // {
                    //     IsPointingAtEnemy = true;
                    //     //Debug.Log(string.Format("Aiming at {0}", hit.collider.gameObject.name));
                    //     break;
                    // }
                }
            }
        }


        // Update various animated features in LateUpdate because it needs to override the animated arm position
        void LateUpdate()
        {
            UpdateWeaponAiming();
            UpdateWeaponBob();
            UpdateWeaponRecoil();
            UpdateWeaponSwitching();

            // Set final weapon socket position based on all the combined animation influences
            weaponRoot.localPosition =
                _weaponLocalPos + _weaponBobLocalPos + _weaponRecoilLocalPos;
        }

        // Sets the FOV of the main camera and the weapon camera simultaneously
        public void SetFov(float fov)
        {
            //m_PlayerCharacterController.PlayerCamera.fieldOfView = fov;
            weaponCamera.fieldOfView = fov * weaponFovMultiplier;
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
                    _weaponLocalPos = weaponPose_Down.localPosition;
                    _weaponReadiness = WeaponReadyState.PutUpNew;
                    ActiveWeaponIndex = _nextWeaponIndex;

                    WeaponController newWeapon = GetWeaponAtSlotIndex(_nextWeaponIndex);
                    if (OnSwitchedToWeapon != null)
                    {
                        OnSwitchedToWeapon.Invoke(newWeapon);
                    }
                }
                // otherwise, remember we are putting down our current weapon for switching to the next one
                else
                {
                    _weaponReadiness = WeaponReadyState.PutDownPrevious;
                }
            }
        }

        // public WeaponController HasWeapon(WeaponController weaponPrefab)
        // {
        //     // Checks if we already have a weapon coming from the specified prefab
        //     for (var index = 0; index < _weaponSlots.Length; index++)
        //     {
        //         var w = _weaponSlots[index];
        //         if (w != null && w.SourcePrefab == weaponPrefab.gameObject)
        //         {
        //             return w;
        //         }
        //     }

        //     return null;
        // }

        // Updates weapon position and camera FoV for the aiming transition
        void UpdateWeaponAiming()
        {
            if (_weaponReadiness == WeaponReadyState.Up)
            {
                WeaponController activeWeapon = GetActiveWeapon();
                if (IsAiming && activeWeapon)
                {
                    _weaponLocalPos = Vector3.Lerp(_weaponLocalPos,
                        weaponPose_Aiming.localPosition + activeWeapon.AimOffset,
                        aimAnimationSpeed * Time.deltaTime);
                    SetFov(Mathf.Lerp(mainCamera.fieldOfView,
                        activeWeapon.AimZoomRatio * defaultFovRef.Value, aimAnimationSpeed * Time.deltaTime));
                }
                else
                {
                    _weaponLocalPos = Vector3.Lerp(_weaponLocalPos,
                        weaponPose_Default.localPosition, aimAnimationSpeed * Time.deltaTime);
                    SetFov(Mathf.Lerp(mainCamera.fieldOfView, defaultFovRef.Value,
                        aimAnimationSpeed * Time.deltaTime));
                }
            }
        }

        // Updates the weapon bob animation based on character speed
        void UpdateWeaponBob()
        {
            if (Time.deltaTime > 0f)
            {
                Vector3 playerCharacterVelocity =
                    (_player.transform.position - _lastPlayerPos) / Time.deltaTime;

                // calculate a smoothed weapon bob amount based on how close to our max grounded movement velocity we are
                float characterMovementFactor = 0f;
                if (_player.IsGrounded)
                {
                    characterMovementFactor =
                        Mathf.Clamp01(playerCharacterVelocity.magnitude /
                                      (_player.MaxSpeedOnGround *
                                       _player.SprintSpeedModifier));
                }

                _weaponBobFactor =
                    Mathf.Lerp(_weaponBobFactor, characterMovementFactor, weaponBobSharpness * Time.deltaTime);

                // Calculate vertical and horizontal weapon bob values based on a sine function
                float bobAmount = IsAiming ? weaponBob_aiming : weaponBob_default;
                float frequency = weaponBobFrequency;
                float hBobValue = Mathf.Sin(Time.time * frequency) * bobAmount * _weaponBobFactor;
                float vBobValue = ((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * bobAmount *
                                  _weaponBobFactor;

                // Apply weapon bob
                _weaponBobLocalPos.x = hBobValue;
                _weaponBobLocalPos.y = Mathf.Abs(vBobValue);

                _lastPlayerPos = _player.transform.position;
            }
        }

        // Updates the weapon recoil animation
        void UpdateWeaponRecoil()
        {
            // if the accumulated recoil is further away from the current position, make the current position move towards the recoil target
            if (_weaponRecoilLocalPos.z >= _totalRecoil.z * 0.99f)
            {
                _weaponRecoilLocalPos = Vector3.Lerp(_weaponRecoilLocalPos, _totalRecoil,
                    recoilSharpness * Time.deltaTime);
            }
            // otherwise, move recoil position to make it recover towards its resting pose
            else
            {
                _weaponRecoilLocalPos = Vector3.Lerp(_weaponRecoilLocalPos, Vector3.zero,
                    recoilRecoverySharpness * Time.deltaTime);
                _totalRecoil = _weaponRecoilLocalPos;
            }
        }

        // Updates the animated transition of switching weapons
        void UpdateWeaponSwitching()
        {
            // Calculate the time ratio (0 to 1) since weapon switch was triggered
            float switchingTimeFactor = 0f;
            if (WeaponSwitchDelay == 0f)
            {
                switchingTimeFactor = 1f;
            }
            else
            {
                switchingTimeFactor = Mathf.Clamp01((Time.time - _weaponSwitchStartTime) / WeaponSwitchDelay);
            }

            // Handle transiting to new switch state
            if (switchingTimeFactor >= 1f)
            {
                if (_weaponReadiness == WeaponReadyState.PutDownPrevious)
                {
                    // Deactivate old weapon
                    WeaponController oldWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
                    if (oldWeapon != null)
                    {
                        oldWeapon.ShowWeapon(false);
                    }

                    ActiveWeaponIndex = _nextWeaponIndex;
                    switchingTimeFactor = 0f;

                    // Activate new weapon
                    WeaponController newWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
                    if (OnSwitchedToWeapon != null)
                    {
                        OnSwitchedToWeapon.Invoke(newWeapon);
                    }

                    if (newWeapon)
                    {
                        _weaponSwitchStartTime = Time.time;
                        _weaponReadiness = WeaponReadyState.PutUpNew;
                    }
                    else
                    {
                        // if new weapon is null, don't follow through with putting weapon back up
                        _weaponReadiness = WeaponReadyState.Down;
                    }
                }
                else if (_weaponReadiness == WeaponReadyState.PutUpNew)
                {
                    _weaponReadiness = WeaponReadyState.Up;
                }
            }

            // Handle moving the weapon socket position for the animated weapon switching
            if (_weaponReadiness == WeaponReadyState.PutDownPrevious)
            {
                _weaponLocalPos = Vector3.Lerp(weaponPose_Default.localPosition,
                    weaponPose_Down.localPosition, switchingTimeFactor);
            }
            else if (_weaponReadiness == WeaponReadyState.PutUpNew)
            {
                _weaponLocalPos = Vector3.Lerp(weaponPose_Down.localPosition,
                    weaponPose_Default.localPosition, switchingTimeFactor);
            }
        }

        // Adds a weapon to our inventory
        public bool AddWeapon(WeaponController weaponPrefab)
        {
            // // if we already hold this weapon type (a weapon coming from the same source prefab), don't add the weapon
            // if (HasWeapon(weaponPrefab) != null)
            // {
            //     return false;
            // }

            // search our weapon slots for the first free one, assign the weapon to it, and return true if we found one. Return false otherwise
            for (int i = 0; i < _weaponSlots.Length; i++)
            {
                // only add the weapon if the slot is free
                if (_weaponSlots[i] == null)
                {
                    // spawn the weapon prefab as child of the weapon socket
                    WeaponController weaponInstance = Instantiate(weaponPrefab, weaponRoot);
                    weaponInstance.transform.localPosition = Vector3.zero;
                    weaponInstance.transform.localRotation = Quaternion.identity;

                    // Set owner to this gameObject so the weapon can alter projectile/damage logic accordingly
                    weaponInstance.Owner = gameObject;
                    weaponInstance.SourcePrefab = weaponPrefab.gameObject;
                    weaponInstance.ShowWeapon(false);

                    // Assign the first person layer to the weapon
                    foreach (Transform t in weaponInstance.gameObject.GetComponentsInChildren<Transform>(true))
                        t.gameObject.layer = fpsWeaponsLayer;

                    _weaponSlots[i] = weaponInstance;

                    // if (OnAddedWeapon != null)
                    //     OnAddedWeapon.Invoke(weaponInstance, i);

                    return true;
                }
            }

            // Handle auto-switching to weapon if no weapons currently
            if (GetActiveWeapon() == null)
            {
                SwitchWeapon(true);
            }

            return false;
        }

        public bool RemoveWeapon(WeaponController weaponInstance)
        {
            // Look through our slots for that weapon
            for (int i = 0; i < _weaponSlots.Length; i++)
            {
                // when weapon found, remove it
                if (_weaponSlots[i] == weaponInstance)
                {
                    _weaponSlots[i] = null;

                    // if (OnRemovedWeapon != null)
                    //     OnRemovedWeapon.Invoke(weaponInstance, i);

                    Destroy(weaponInstance.gameObject);

                    // Handle case of removing active weapon (switch to next weapon)
                    if (i == ActiveWeaponIndex)
                    {
                        SwitchWeapon(true);
                    }

                    return true;
                }
            }

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
        }        
    }
}