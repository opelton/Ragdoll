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
        [SerializeField] private IntReference playerMaxAmmo;

        void OnEnable()
        {
            UpdateMaxAmmo(playerMaxAmmo.Value);
            UpdateCurrentAmmo(playerCurrentAmmo.Value);
        }

        public void UpdateCurrentAmmo(int newCurrentAmmo)
        {
            currentAmmoDisplay.text = newCurrentAmmo.ToString();
        }

        public void UpdateMaxAmmo(int newMaxAmmo)
        {
            // hide display if max ammo is 0
            if(ammoDisplayRoot.activeSelf != (newMaxAmmo != 0))
                ammoDisplayRoot.SetActive(newMaxAmmo != 0);

            maxAmmoDisplay.text = newMaxAmmo.ToString();
        }
    }
}