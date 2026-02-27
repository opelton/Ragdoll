using Potato.Core;
using UnityEngine;

namespace Potato.Game.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject titleText;
        [SerializeField] private GameObject mainButtons;

        [Header("Data")]
        [SerializeField] private BoolReference showSettingsRef;

        void Start()
        {
            showSettingsRef.Value = false;
        }

        public void ToggleSettingsPanel()
        {
            showSettingsRef.Value = !showSettingsRef.Value;
        }

        public void OnSettingsVisibilityChanged(bool settingsVisible)
        {
            titleText.SetActive(!settingsVisible);
            mainButtons.SetActive(!settingsVisible);
        }
    }
}