using UnityEngine;
using Potato.Core;

// manages UI that isn't owned by specific scenes or game objects
// I'm rusty at UI, so this will be tightly bound and a little ugly at first
namespace Potato.Game.UI
{
    public class CommonUiController : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject settingsCanvas;
        [SerializeField] private GameObject playerHudCanvas;
        [SerializeField] private GameObject loadingCanvas;
        [SerializeField] private GameObject debugCanvas;
        [SerializeField] private SettingsPanelController settingsPanel;

        [Header("Shared Data")]
        [SerializeField] private BoolReference showSettingsRef;
        [SerializeField] private BoolReference showHudRef;
        [SerializeField] private BoolReference showLoadingRef;
        [SerializeField] private BoolReference showDebugRef;
        [SerializeField] private BoolReference isPausedRef;

        void Start()
        {
            SetSettingsVisibility(showSettingsRef.Value);
            SetHudVisibility(showHudRef.Value);
            SetLoadingVisibility(showLoadingRef.Value);
            SetDebugVisibility(showDebugRef.Value);
        }

        public void TryUnpause()
        {
            if(isPausedRef.Value)
                isPausedRef.Value = false;
            else
                showSettingsRef.Value = false;
        }

        public void OnPauseChanged(bool isPaused) => showSettingsRef.Value = isPaused;
        public void SetSettingsVisibility(bool visible) => settingsCanvas.SetActive(visible);
        public void SetHudVisibility(bool visible) => playerHudCanvas.SetActive(visible);
        public void SetLoadingVisibility(bool visible) => loadingCanvas.SetActive(visible);
        public void SetDebugVisibility(bool visible) => debugCanvas.SetActive(visible);
    }
}