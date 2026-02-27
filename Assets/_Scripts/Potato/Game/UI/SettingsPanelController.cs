using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Potato.Core;
using TMPro;

namespace Potato.Game.UI
{
    public class SettingsPanelController : MonoBehaviour
    {
        [Header("Settings widgets")]
        [SerializeField] private Toggle muteToggle;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Slider mouseSensitivitySlider;
        [SerializeField] private Toggle fullScreenToggle;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle showFramerateToggle;

        [Header("Gameplay specific")]
        [SerializeField] private TMP_Text settingsTitle;
        [SerializeField] private GameObject restartButton;
        [SerializeField] private GameObject quitButton;

        [Header("Shared data")]
        [SerializeField] private BoolReference muteRef;
        [SerializeField] private FloatReference volumeRef;
        [SerializeField] private FloatReference mouseSensitivityRef;
        [SerializeField] private BoolReference fullscreenRef;
        [SerializeField] private Vector2IntReference resolutionRef;
        [SerializeField] private BoolReference showFramerateRef;
        [SerializeField] private GameStateReference gameStateRef;

        private Vector2Int[] _allResolutions = null;
        private Dictionary<Vector2Int, int> _resolutionLookup;

        void OnEnable()
        {
            SetGameplayOptionsVisibility(gameStateRef.Value.SettingsPanelMode);
#if !UNITY_WEBGL
            if(_allResolutions == null)
            {
                _resolutionLookup = new();
                _allResolutions = SettingsBridge.GetValidScreenResolutions();
                for(int i = 0; i < _allResolutions.Length; ++i)
                    _resolutionLookup.TryAdd(_allResolutions[i], i);

                PopulateResolutionDropdown();
            }

            resolutionDropdown.gameObject.SetActive(true);
#else
            resolutionDropdown.gameObject.SetActive(false);
#endif
            SyncWidgetsToData();
        }

        void SetGameplayOptionsVisibility(bool showGameplayOptions)
        {
            settingsTitle.text = showGameplayOptions ? "PAUSE / SETTINGS" : "SETTINGS";
            restartButton.SetActive(showGameplayOptions);
            quitButton.SetActive(showGameplayOptions);
        }

        void PopulateResolutionDropdown()
        {
            var optionsList = new List<string>();
            foreach(var resolution in _allResolutions)
                optionsList.Add($"{resolution.x}x{resolution.y}");
            
            resolutionDropdown.AddOptions(optionsList);
        }

        void SyncWidgetsToData()
        {
            muteToggle.isOn = muteRef.Value;
            volumeSlider.value = volumeRef.Value;
            mouseSensitivitySlider.value = mouseSensitivityRef.Value;
            fullScreenToggle.isOn = fullscreenRef.Value;

#if !UNITY_WEBGL
            resolutionDropdown.value = _resolutionLookup[resolutionRef.Value];
#endif
            showFramerateToggle.isOn = showFramerateRef.Value;
        }

        public void OnVolumeSliderChanged(float newValue) => volumeRef.Value = newValue;
        public void OnMouseSliderChanged(float newValue) => mouseSensitivityRef.Value = newValue;
        public void OnResolutionDropdownChanged(int newIndex) => resolutionRef.Value = _allResolutions[newIndex];
        public void OnFullscreenToggleChanged(bool isToggled) => fullscreenRef.Value = isToggled;
        public void OnMuteToggleChanged(bool isToggled) => muteRef.Value = isToggled;
        public void OnFramerateToggleChanged(bool isToggled) => showFramerateRef.Value = isToggled;
        public void OnGameStateChanged(GameState state) => SetGameplayOptionsVisibility(state.SettingsPanelMode);
    }
}