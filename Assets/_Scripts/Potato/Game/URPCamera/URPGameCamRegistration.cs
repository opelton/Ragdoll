using UnityEngine;

namespace Potato.Game
{
    public class URPGameCamRegistration : MonoBehaviour
    {
        [SerializeField] private Camera gameCam;
        [SerializeField] private Camera fpsCam;
        [SerializeField] private GameplayCameraDataReference gameCamsDataRef;

        void OnEnable()
        {
            var gameCamsData = new GameplayCameraData
            {
                gameplayCamera = gameCam,
                fpsCamera = fpsCam
            };
            gameCamsDataRef.Value = gameCamsData;
        }

        void OnDisable()
        {
            var gameCamsData = new GameplayCameraData();
            gameCamsDataRef.Value = gameCamsData;
        }
    }
}