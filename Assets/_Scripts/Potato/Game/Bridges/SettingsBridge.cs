using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

using Potato.Core;

namespace Potato.Game
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
        public int Fov;
        public int TargetFps;
        public bool LockFps;
        public bool Vsync;
    }

    public class SettingsBridge : MonoBehaviour
    {
        static readonly string SettingsKey = "MainSettings";

        [Header("Out Data")]
        [SerializeField] FloatReference MouseSensitivity = .6f;
        [SerializeField] FloatReference MainVolume = .6f;
        [SerializeField] BoolReference MuteAudio = false;
        [SerializeField] BoolReference ShowFPS = false;
        [SerializeField] BoolReference FullScreen = false;
        [SerializeField] Vector2IntReference Resolution = new Vector2Int(1280, 720);
        [SerializeField] IntReference Fov = 60;
        [SerializeField] IntReference TargetFps = 60;
        [SerializeField] BoolReference LockFps = true;
        [SerializeField] BoolReference Vsync = true;

        [Header("Audio")]
        [SerializeField] AudioMixer MainAudioMixer;

        void Start()
        {
            SettingsData data;
            if (PlayerPrefs.HasKey(SettingsKey))
            {
                string settingsJson = PlayerPrefs.GetString(SettingsKey);
                data = JsonUtility.FromJson<SettingsData>(settingsJson);
            }
            else
            {
                data = SystemToSettingsData();
            }

            // webGL can't fullscreen on init, must be triggered by runtime user input
#if UNITY_WEBGL
            data.FullScreen = false;
#endif
            ApplySettingsData(data);
        }

        void OnApplicationQuit()
        {
            HandleSaveSettingsCommand();
        }

        public void HandleSaveSettingsCommand()
        {
            SettingsData data = ToSettingsData();
            PlayerPrefs.SetString(SettingsKey, JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }

        SettingsData SystemToSettingsData()
        {
            // hardcode the settings that are basically made-up and don't actually have a unity input
            return new SettingsData
            {
                MouseSensitivity = .6f,
                MainVolume = TryGetCurrentVolume(),
                MuteAudio = false,
                ShowFPS = false,
                FullScreen = Screen.fullScreen,
                Resolution = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height),
                Fov = 60,
                TargetFps = Application.targetFrameRate,
                LockFps = Application.targetFrameRate != -1,
                Vsync = QualitySettings.vSyncCount != 0
            };
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
                Resolution = Resolution.Value,
                Fov = Fov.Value,
                TargetFps = TargetFps.Value,
                LockFps = LockFps.Value,
                Vsync = Vsync.Value
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
            Fov.Value = data.Fov;
            TargetFps.Value = data.TargetFps;
            LockFps.Value = data.LockFps;
            Vsync.Value = data.Vsync;
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
            if (!MuteAudio.Value && rawValue > 0f)
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

        public void SetFullScreen(bool fullScreen)
        {
#if UNITY_WEBGL
            unityInstance.SetFullScreen(fullScreen ? 1 : 0); 
#else
            Screen.fullScreen = fullScreen;
#endif
        }

        public void SetResolution(Vector2Int resolution) => Screen.SetResolution(resolution.x, resolution.y, FullScreen.Value);

        public void SetFramerateCap(int fpsCap) => FramerateCap(LockFps.Value, fpsCap);
        public void SetFramerateLock(bool locked) => FramerateCap(locked, TargetFps.Value);
        public void SetVsync(bool vsync) => QualitySettings.vSyncCount = vsync ? 1 : 0;

        void FramerateCap(bool locked, int fpsCap) => Application.targetFrameRate = locked ? fpsCap : -1;

        static float CurrentAspectRatio() => (float)Screen.currentResolution.height / (float)Screen.currentResolution.width;

        static bool ResolutionMatchesAspect(Vector2Int resolution)
        {
            float aspect = (float)resolution.y / (float)resolution.x;
            const float deltaF = 0.01f;

            return Mathf.Abs(CurrentAspectRatio() - aspect) < deltaF;
        }

        public static Vector2Int[] GetValidScreenResolutions()
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

            return validResolutions.ToArray();
        }

        public static Vector2Int GetCurrentResolution()
        {
            return new(Screen.currentResolution.width, Screen.currentResolution.height);
        }
    }
}