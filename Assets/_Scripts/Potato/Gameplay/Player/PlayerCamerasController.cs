using UnityEngine;
using Potato.Core;

namespace Potato.Gameplay
{
    public class PlayerCamerasController : MonoBehaviour
    {
        [SerializeField] private Camera gameCam;
        [SerializeField] private Camera fpsCam;
        [SerializeField] private PlayerStance playerStance;
        [SerializeField] private IntReference baseFov;
        [SerializeField] private int fixedFirstPersonFov;

        public Vector3 AimDir => fpsCam.transform.forward;
        public Vector3 AimPos => fpsCam.transform.position;
        public Transform AimTransform => fpsCam.transform;

        void Start()
        {
            UpdateFOVModifier(playerStance.FOVModifier.Value);
            fpsCam.fieldOfView = fixedFirstPersonFov;
            playerStance.FOVModifier.OnValueChanged += UpdateFOVModifier;
        }

        void UpdateFOVModifier(float mod)
        {
            gameCam.fieldOfView = baseFov.Value * mod;
        }

        public void UpdateBaseFOV(int fov)
        {
            gameCam.fieldOfView = fov * playerStance.FOVModifier.Value;
        }

        public void ReparentCameras(Transform newParent)
        {
            gameCam.transform.SetParent(newParent);
            fpsCam.transform.SetParent(newParent);
        }
    }
}