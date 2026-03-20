using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Potato.Game
{
    // URP cameras don't have a depth value, and determine order by stacking
    public class URPCameraStacker : MonoBehaviour
    {
        // base camera when no gameplay cameras exist
        [SerializeField] private Camera uiCamera;
        [SerializeField] private GameplayCameraDataReference gameCameraData;

        void Start() => OnGameplayCameraDataChanged();

        void SetGameplayCameraStack()
        {
            var uiCamData = uiCamera.GetComponent<UniversalAdditionalCameraData>();
            var gameCamData = gameCameraData.Value.gameplayCamera.GetComponent<UniversalAdditionalCameraData>();
            var fpsCamData = gameCameraData.Value.fpsCamera.GetComponent<UniversalAdditionalCameraData>();

            if(uiCamData.renderType == CameraRenderType.Base)
                uiCamData.cameraStack.Clear();

            gameCamData.renderType = CameraRenderType.Base;
            fpsCamData.renderType = CameraRenderType.Overlay;
            uiCamData.renderType = CameraRenderType.Overlay;

            gameCamData.cameraStack.Clear();
            gameCamData.cameraStack.Add(gameCameraData.Value.fpsCamera);
            gameCamData.cameraStack.Add(uiCamera);
        }

        void SetUiCameraStack()
        {
            var uiCamData = uiCamera.GetComponent<UniversalAdditionalCameraData>();
            uiCamData.renderType = CameraRenderType.Base;
            uiCamData.cameraStack.Clear();
        }

        // ignoring this arg, I just want it to be more obvious in the UnityEvent ui
        public void OnGameplayCameraDataChanged(GameplayCameraData _ = null)
        {
            if(gameCameraData.Value != null && gameCameraData.Value.gameplayCamera != null && gameCameraData.Value.fpsCamera != null)
                SetGameplayCameraStack();
            else
                SetUiCameraStack();
        }
    }
}