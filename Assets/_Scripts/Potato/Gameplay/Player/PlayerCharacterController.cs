using Potato.Core;
using UnityEngine;

// todo -- rewire all inputs
// input context needs to update on scene load/transition
namespace Potato.Gameplay
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerCharacterController : MonoBehaviour
    {
        [Header("Main Camera")]
        [SerializeField] private Camera playerCamera;

        [Header("Input Data")]
        [SerializeField] private Vector2Reference moveInput;
        [SerializeField] private Vector2Reference lookInput;

        [Header("Tuning")]
        [SerializeField] private float turnSpeed = 1000f;
        [SerializeField] private float moveSpeed = 10f;


        // --
        private CharacterController _controller;
        private float _cameraY = 0;

        void Start()
        {
            _controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            float dt = Time.deltaTime;

            // camera x
            transform.Rotate(0f, lookInput.Value.x * turnSpeed * dt, 0f);
            
            // camera y
            _cameraY += lookInput.Value.y * turnSpeed * dt;
            _cameraY = Mathf.Clamp(_cameraY, -89f, 89f);
            playerCamera.transform.localEulerAngles = new Vector3(_cameraY, 0, 0);

            // move
            Vector3 worldspaceMoveInput = transform.TransformVector(moveInput.Value.x, 0, moveInput.Value.y);
            _controller.Move(dt * moveSpeed * worldspaceMoveInput);
        }
    }
}