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
        [SerializeField] private UiSliderControl volumeSlider;
        [SerializeField] private UiSliderControl mouseSensitivitySlider;
        [SerializeField] private UiSliderControl fovSlider;
        [SerializeField] private UiSliderControl fpsCapSlider;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Text resolutionText;
        [SerializeField] private UiToggleControl muteToggle;
        [SerializeField] private UiToggleControl fullScreenToggle;
        [SerializeField] private UiToggleControl showFramerateToggle;
        [SerializeField] private UiToggleControl lockFpsToggle;
        [SerializeField] private UiToggleControl vsyncToggle;

        [Header("Gameplay specific")]
        [SerializeField] private TMP_Text settingsTitle;
        [SerializeField] private GameObject restartButton;
        [SerializeField] private GameObject mainMenuButton;
        [SerializeField] private GameObject quitButton;

        [Header("Shared data")]
        [SerializeField] private BoolReference muteRef;
        [SerializeField] private FloatReference volumeRef;
        [SerializeField] private FloatReference mouseSensitivityRef;
        [SerializeField] private BoolReference fullscreenRef;
        [SerializeField] private Vector2IntReference resolutionRef;
        [SerializeField] private BoolReference showFramerateRef;
        [SerializeField] private GameStateReference gameStateRef;
        [SerializeField] private IntReference fovRef;
        [SerializeField] private IntReference targetFramerateRef;
        [SerializeField] private BoolReference lockFramerateRef;
        [SerializeField] private BoolReference vsyncRef;

        private Vector2Int[] _allResolutions = null;

#if !UNITY_WEBGL && !UNITY_EDITOR
        private Dictionary<Vector2Int, int> _resolutionLookup;
#endif

        void Start()
        {
            SetGameplayOptionsVisibility(gameStateRef.Value.ShowGameFlowSettingsButtons);

#if UNITY_WEBGL || UNITY_EDITOR
            _allResolutions = new[] { SettingsBridge.GetCurrentResolution()};

            // no resolution or fullscreen in editor or webgl
            resolutionDropdown.interactable = false;
            resolutionText.color = Color.grey;
            fullScreenToggle.SetInteractable(false);

            // vsync overridden in editor and webgl
            vsyncToggle.SetInteractable(false);
#else
            if(_allResolutions == null)
            {
                _resolutionLookup = new();
                _allResolutions = SettingsBridge.GetValidScreenResolutions();
                for(int i = 0; i < _allResolutions.Length; ++i)
                    _resolutionLookup.TryAdd(_allResolutions[i], i);

            }
#endif
            PopulateResolutionDropdown();

#if UNITY_WEBGL
            // no quit in webgl
            quitButton.SetActive(false);

            // vsync being forced-on overrides target fps
            lockFpsToggle.SetInteractable(false);
#else
            quitButton.SetActive(true);
#endif
            SyncWidgetsToData();
        }

        void SetGameplayOptionsVisibility(bool showGameplayOptions)
        {
            settingsTitle.text = showGameplayOptions ? "PAUSE / SETTINGS" : "SETTINGS";
            restartButton.SetActive(showGameplayOptions);
            mainMenuButton.SetActive(showGameplayOptions);
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
            muteToggle.Value = muteRef.Value;
            volumeSlider.Value = volumeRef.Value;
            mouseSensitivitySlider.Value = mouseSensitivityRef.Value;
            showFramerateToggle.Value = showFramerateRef.Value;
            fovSlider.Value = fovRef.Value;
            fpsCapSlider.Value = targetFramerateRef.Value;

#if UNITY_WEBGL || UNITY_EDITOR
            fullScreenToggle.Value = false;
            resolutionDropdown.value = 0;
#else
            vsyncToggle.Value = vsyncRef.Value;
            fullScreenToggle.Value = fullscreenRef.Value;
            resolutionDropdown.value = _resolutionLookup[resolutionRef.Value];
#endif

            // vsync is forced-on in webgl, forced-off in editor (which also impacts target framerate)
#if UNITY_WEBGL
            vsyncToggle.Value = true;
            lockFpsToggle.Value = false;
#elif UNITY_EDITOR
            vsyncToggle.Value = false;
#else
            lockFpsToggle.Value = lockFramerateRef.Value;
            vsyncToggle.Value = vsyncRef.Value;
#endif

            volumeSlider.SetInteractable(!muteRef.Value);
            fpsCapSlider.SetInteractable(!vsyncRef.Value && lockFramerateRef.Value);
        }

        public void OnVolumeSliderChanged(float newValue) => volumeRef.Value = newValue;
        public void OnMouseSliderChanged(float newValue) => mouseSensitivityRef.Value = newValue;
        public void OnResolutionDropdownChanged(int newIndex) => resolutionRef.Value = _allResolutions[newIndex];
        public void OnFullscreenToggleChanged(bool isToggled) => fullscreenRef.Value = isToggled;
        public void OnMuteToggleChanged(bool isToggled)
        {
            muteRef.Value = isToggled;
            volumeSlider.SetInteractable(!isToggled);
        }
        public void OnFramerateToggleChanged(bool isToggled) => showFramerateRef.Value = isToggled;
        public void OnFovSliderChanged(float fov) => fovRef.Value = (int)fov;
        public void OnGameStateChanged(GameState state) => SetGameplayOptionsVisibility(state.ShowGameFlowSettingsButtons);
        public void OnFpsCapSliderChanged(float newValue) => targetFramerateRef.Value = (int)newValue;
        public void OnFpsCapToggleChanged(bool isToggled)
        {
            lockFramerateRef.Value = isToggled;

            // fps cap must disable vsync to work, ui should reflect that
            if(isToggled && vsyncRef.Value)
                vsyncToggle.Value = false;

            fpsCapSlider.SetInteractable(!vsyncRef.Value && isToggled);
        }
        public void OnVsyncToggleChanged(bool isToggled)
        {
            // engine prioritizes vsync over target framerate
            vsyncRef.Value = isToggled;

            // enabling vsync should also disable the framerate lock in settings
            if(isToggled && lockFramerateRef.Value)
                lockFpsToggle.Value = false;

            fpsCapSlider.SetInteractable(!vsyncRef.Value && lockFramerateRef.Value);
        }
    }
}