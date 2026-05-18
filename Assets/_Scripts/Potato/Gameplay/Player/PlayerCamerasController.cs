using UnityEngine;
using Potato.Core;

namespace Potato.Gameplay
{
    public class PlayerCamerasController : MonoBehaviour
    {
        [SerializeField] private Camera gameCam;
        [SerializeField] private Camera fpsCam;
        [SerializeField] private IntReference baseFov;
        [SerializeField] private FloatReference stanceFovModifier;
        [SerializeField] private int fixedFirstPersonFov;

        public Vector3 AimDir => fpsCam.transform.forward;
        public Vector3 AimPos => fpsCam.transform.position;
        public Transform AimTransform => fpsCam.transform;

        void Start() => UpdateFovs();

        public void UpdateFovs()
        {
            gameCam.fieldOfView = (float)baseFov.Value * stanceFovModifier.Value;
            fpsCam.fieldOfView = fixedFirstPersonFov;
        }
    }
}