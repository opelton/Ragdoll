using UnityEngine;
using UnityEngine.UI;
using Potato.Core;

namespace Potato.Gameplay.UI
{
    public class PlayerCrosshairUI : MonoBehaviour
    {
        [SerializeField] private WeaponReference playerActiveWeapon;
        [SerializeField] private BoolReference isPlayerTargetingEnemy;
        [SerializeField] private Image crosshairImage;
        [SerializeField] private Sprite defaultCrosshairSprite;
        [SerializeField] private int defaultCrosshairSize;
        [SerializeField] private Color crosshairColor_Neutral = Color.white;
        [SerializeField] private Color crosshairColor_Hostile = Color.red;
        RectTransform _crosshairTransform;

        void Awake()
        {
            if(_crosshairTransform == null)
                _crosshairTransform = crosshairImage.GetComponent<RectTransform>();
            UpdateCrosshairSprite();
        }

        void Update()
        {
            crosshairImage.color = isPlayerTargetingEnemy.Value ? crosshairColor_Hostile : crosshairColor_Neutral;
        }

        void UpdateCrosshairSprite()
        {
            if(playerActiveWeapon.Value == null)
            {
                crosshairImage.sprite = defaultCrosshairSprite;
                _crosshairTransform.sizeDelta = Vector2.one * defaultCrosshairSize;
            }
            else
            {
                crosshairImage.sprite = playerActiveWeapon.Value.WeaponCrosshairData.CrosshairSprite;
                _crosshairTransform.sizeDelta = Vector2.one * playerActiveWeapon.Value.WeaponCrosshairData.CrosshairSize;
            }
        }

        public void OnPlayerActiveWeaponChanged(WeaponController _)
        {
            UpdateCrosshairSprite();
        }
    }
}