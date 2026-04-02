using UnityEngine;
using Potato.Core;
using TMPro;

namespace Potato.Gameplay.UI
{
    public class AmmoUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject ammoDisplayRoot;
        [SerializeField] private TMP_Text currentAmmoDisplay;
        [SerializeField] private TMP_Text maxAmmoDisplay;
        
        [Header("Shared Data")]
        [SerializeField] private IntReference playerCurrentAmmo;        
        [SerializeField] private WeaponReference activeWeaponRef;

        void OnEnable()
        {
            UpdateActiveWeapon(activeWeaponRef.Value);
            UpdateCurrentAmmo(playerCurrentAmmo.Value);
        }

        public void UpdateCurrentAmmo(int newCurrentAmmo)
        {
            currentAmmoDisplay.text = newCurrentAmmo.ToString();
        }

        public void UpdateActiveWeapon(WeaponController activeWeapon)
        {
            // hide display if max ammo is 0, check for change before setting
            if(ammoDisplayRoot.activeSelf != (activeWeapon != null))
                ammoDisplayRoot.SetActive(activeWeapon != null);

            if(activeWeapon != null)
                maxAmmoDisplay.text = activeWeapon.MaxAmmo.ToString();
        }
    }
}