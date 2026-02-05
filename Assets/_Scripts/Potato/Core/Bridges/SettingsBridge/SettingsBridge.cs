using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

using Potato.Core;

namespace Core.Potato
{
    [Serializable]
    public class SettingsData
    {
        public float MouseSensitivity;
        public float MainVolume;
        public bool MuteAudio;
        public bool ShowFPS;
        public bool FullScreen;
        public Vector2Int Resolution;
    }

    // todo -- aspect ratio? fov?
    public class SettingsBridge : MonoBehaviour
    {
        static readonly string SettingsKey = "MainSettings";

        [Header("Out Data")]
        [SerializeField] FloatReference MouseSensitivity = .6f;
        [SerializeField] FloatReference MainVolume = .6f;
        [SerializeField] BoolReference MuteAudio = false;
        [SerializeField] BoolReference ShowFPS = false;
        [SerializeField] BoolReference FullScreen = false;
#if !UNITY_WEBGL
        // webGL browser play is basically just fullscreen on/off, don't even offer resolution control
        [SerializeField] Vector2IntReference Resolution = new Vector2Int(1280, 720);
#endif

        [Header("Audio")]
        [SerializeField] AudioMixer MainAudioMixer;

        void Start()
        {
            if (PlayerPrefs.HasKey(SettingsKey))
            {
                string settingsJson = PlayerPrefs.GetString(SettingsKey);
                SettingsData data = JsonUtility.FromJson<SettingsData>(settingsJson);
                ApplySettingsData(data);
            }
            else
            {
                InitializeFromSystem();
            }
        }

        void InitializeFromSystem()
        {
            // hardcode the settings that are basically made-up and don't actually have a unity input
            MouseSensitivity.Value = .6f;
            MainVolume.Value = TryGetCurrentVolume();
            MuteAudio.Value = false;
            ShowFPS.Value = false;
            FullScreen.Value = Screen.fullScreen;
            Resolution.Value = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.width);
        }

        public void HandleSaveSettingsCommand()
        {
            SettingsData data = ToSettingsData();
            PlayerPrefs.SetString(SettingsKey, JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }

        SettingsData ToSettingsData()
        {
            return new SettingsData
            {
                MouseSensitivity = MouseSensitivity.Value,
                MainVolume = MainVolume.Value,
                MuteAudio = MuteAudio.Value,
                ShowFPS = ShowFPS.Value,
                FullScreen = FullScreen.Value,
                Resolution = Resolution.Value
            };
        }

        void ApplySettingsData(SettingsData data)
        {
            MouseSensitivity.Value = data.MouseSensitivity;
            MainVolume.Value = data.MainVolume;
            MuteAudio.Value = data.MuteAudio;
            ShowFPS.Value = data.ShowFPS;
            FullScreen.Value = data.FullScreen;
            Resolution.Value = data.Resolution;
        }

        float TryGetCurrentVolume()
        {
            if (MainAudioMixer.GetFloat("MasterVolume", out float dB))
                dB = Mathf.Pow(10f, dB / 20f);  // inverse of float->dB
            return dB;
        }

        public void SetVolume(float rawValue)
        {
            float dB = -80f;    // muted value
            if (rawValue > 0f)
            {
                float clampedValue = Mathf.Clamp(rawValue, 0.0001f, 1f);
                dB = 20 * Mathf.Log10(clampedValue);
            }
            MainAudioMixer.SetFloat("MasterVolume", dB);
        }

        public void SetMute(bool muted)
        {
            if (muted)
                SetVolume(0f);
            else
                SetVolume(MainVolume.Value);
        }

        public void SetFullScreen(bool fullScreen) => Screen.fullScreen = fullScreen;

        public void SetResolution(Vector2Int resolution) => Screen.SetResolution(resolution.x, resolution.y, FullScreen.Value);

        static float CurrentAspectRatio() => (float)Screen.currentResolution.height / (float)Screen.currentResolution.width;

        static bool ResolutionMatchesAspect(Vector2Int resolution)
        {
            float aspect = (float)resolution.y / (float)resolution.x;
            const float deltaF = 0.01f;

            return Mathf.Abs(CurrentAspectRatio() - aspect) < deltaF;
        }

        public static List<Vector2Int> GetValidScreenResolutions()
        {
            List<Vector2Int> validResolutions = new();
            Vector2Int prevRes = Vector2Int.zero;
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                Vector2Int resolution = new(Screen.resolutions[i].width, Screen.resolutions[i].height);

                if (resolution != prevRes && ResolutionMatchesAspect(resolution))
                {
                    validResolutions.Add(resolution);
                    prevRes = resolution;
                }
            }

            return validResolutions;
        }
    }
}