using UnityEngine;
using UnityEngine.UI;
using Potato.Core;

namespace Potato.Gameplay.UI
{
    public class PlayerHealthBarUI : MonoBehaviour
    {
        [SerializeField] private Image HealthFillImage;
        [SerializeField] private IntReference playerHealth;

        void Update()
        {
            HealthFillImage.fillAmount = playerHealth.Value / 100f;
        }
    }
}