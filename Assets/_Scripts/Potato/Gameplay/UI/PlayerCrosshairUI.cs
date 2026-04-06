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

        void Start()
        {
            _crosshairTransform = crosshairImage.GetComponent<RectTransform>();
            UpdateCrosshairSprite();
        }

        void Update()
        {
            crosshairImage.color = isPlayerTargetingEnemy.Value ? crosshairColor_Hostile : crosshairColor_Neutral;
        }

        // void Update()
        // {
        //     UpdateCrosshairPointingAtEnemy(false);
        //     m_WasPointingAtEnemy = m_WeaponsManager.IsPointingAtEnemy;
        // }

        // void UpdateCrosshairPointingAtEnemy(bool force)
        // {
        //     if (m_CrosshairDataDefault.CrosshairSprite == null)
        //         return;

        //     if ((force || !m_WasPointingAtEnemy) && m_WeaponsManager.IsPointingAtEnemy)
        //     {
        //         m_CurrentCrosshair = m_CrosshairDataTarget;
        //         CrosshairImage.sprite = m_CurrentCrosshair.CrosshairSprite;
        //         //m_CrosshairRectTransform.sizeDelta = m_CurrentCrosshair.CrosshairSize * Vector2.one;
        //     }
        //     else if ((force || m_WasPointingAtEnemy) && !m_WeaponsManager.IsPointingAtEnemy)
        //     {
        //         m_CurrentCrosshair = m_CrosshairDataDefault;
        //         CrosshairImage.sprite = m_CurrentCrosshair.CrosshairSprite;
        //         //m_CrosshairRectTransform.sizeDelta = m_CurrentCrosshair.CrosshairSize * Vector2.one;
        //     }

        //     CrosshairImage.color = m_CurrentCrosshair.CrosshairColor;
        //     // CrosshairImage.color = Color.Lerp(CrosshairImage.color, m_CurrentCrosshair.CrosshairColor,
        //     //     Time.deltaTime * CrosshairUpdateshrpness);

        //     // m_CrosshairRectTransform.sizeDelta = Mathf.Lerp(m_CrosshairRectTransform.sizeDelta.x,
        //     //     m_CurrentCrosshair.CrosshairSize,
        //     //     Time.deltaTime * CrosshairUpdateshrpness) * Vector2.one;
        // }

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