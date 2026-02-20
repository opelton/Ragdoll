using UnityEngine;

namespace Potato.Game.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject titleText;
        [SerializeField] private GameObject mainButtons;
        [SerializeField] private GameObject settingsPanel;

        void Start()
        {
            titleText.SetActive(true);
            mainButtons.SetActive(true);
            settingsPanel.SetActive(false);
        }

        public void ToggleSettingsPanel()
        {
            titleText.SetActive(!titleText.activeSelf);
            mainButtons.SetActive(!mainButtons.activeSelf);
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }
}