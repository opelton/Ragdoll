using Potato.Core;
using UnityEngine;

namespace Potato.Gameplay
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerCharacterController : MonoBehaviour
    {
        const float k_JumpGroundingPreventionTime = 0.2f;
        const float k_GroundCheckDistanceInAir = 0.07f;

        [Header("Main Camera")]
        [SerializeField] private Camera playerCamera;

        [Header("Input Data")]
        [SerializeField] private Vector2Reference moveInput;
        [SerializeField] private Vector2Reference lookInput;

        [Header("Gravity")]
        [SerializeField] private float gravityDownForce = 20f;
        [SerializeField] private LayerMask groundCheckLayers;
        [SerializeField] private float groundCheckDistance = 0.05f;

        [Header("Movement")]
        [SerializeField] private float sprintSpeedModifier = 1.5f;
        [SerializeField] private float maxGroundSpeed = 13f;
        [SerializeField] private float groundTurningSharpness = 15f;
        [SerializeField] private float maxAirSpeed = 10f;
        [SerializeField] private float airAcceleration = 25f;
        [SerializeField] private float turnSpeed = 1000f;
        [SerializeField] private float jumpForce = 9f;

        // --
        private CharacterController _controller;
        private Vector3 _velocity = Vector3.zero;
        private float _cameraY = 0;
        private bool _isGrounded = false;
        private Vector3 _groundNormal = Vector3.up;
        private float _lastJumpTime = 0f;

        // inputs
        private bool _sprintInput = false;
        private bool _jumpInput = false;


        // --
        public bool IsGrounded => _isGrounded;

        void Start()
        {
            _controller = GetComponent<CharacterController>();
            _controller.enableOverlapRecovery = true;
        }

        void Update()
        {
            bool wasGrounded = _isGrounded;
            GroundCheck();
            CheckImpact(wasGrounded);
            HandleMovement(Time.deltaTime);
        }

        void HandleMovement(float dt)
        {
            // camera x
            transform.Rotate(0f, lookInput.Value.x * turnSpeed * dt, 0f);

            // camera y
            _cameraY += lookInput.Value.y * turnSpeed * dt;
            _cameraY = Mathf.Clamp(_cameraY, -89f, 89f);
            playerCamera.transform.localEulerAngles = new Vector3(_cameraY, 0, 0);

            // bool isSprinting = m_InputHandler.GetSprintInputHeld();
            // if (isSprinting)
            // {
            //     isSprinting = SetCrouchingState(false, false);
            // }

            float speedModifier = _sprintInput ? sprintSpeedModifier : 1f;
            Vector3 worldspaceMoveInput = transform.TransformVector(moveInput.Value.x, 0f, moveInput.Value.y);

            if (_isGrounded)
            {
                Vector3 targetVelocity = maxGroundSpeed * speedModifier * worldspaceMoveInput;

                // if (IsCrouching)
                //     targetVelocity *= MaxSpeedCrouchedRatio;
                // targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, m_GroundNormal) *
                //                  targetVelocity.magnitude;

                _velocity = Vector3.Lerp(_velocity, targetVelocity, groundTurningSharpness * dt);

                if (_isGrounded && _jumpInput)
                {
                    // force the crouch state to false
                    //if (SetCrouchingState(false, false))
                    {
                        _velocity = new Vector3(_velocity.x, 0f, _velocity.z);
                        _velocity += Vector3.up * jumpForce;

                        // AudioSource.PlayOneShot(JumpSfx);

                        // remember last time we jumped because we need to prevent snapping to ground for a short time
                        _lastJumpTime = Time.time;

                        // Force grounding to false
                        _isGrounded = false;
                        _groundNormal = Vector3.up;
                    }
                }

                // // footsteps sound
                // float chosenFootstepSfxFrequency =
                //     (isSprinting ? FootstepSfxFrequencyWhileSprinting : FootstepSfxFrequency);
                // if (m_FootstepDistanceCounter >= 1f / chosenFootstepSfxFrequency)
                // {
                //     m_FootstepDistanceCounter = 0f;
                //     AudioSource.PlayOneShot(FootstepSfx);
                // }

                // // keep track of distance traveled for footsteps sound
                // m_FootstepDistanceCounter += CharacterVelocity.magnitude * Time.deltaTime;
            }
            // handle air movement
            else
            {
                // add air acceleration
                _velocity += worldspaceMoveInput * airAcceleration * dt;

                // limit air speed to a maximum, but only horizontally
                float verticalVelocity = _velocity.y;
                Vector3 horizontalVelocity = Vector3.ProjectOnPlane(_velocity, Vector3.up);
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxAirSpeed * speedModifier);
                _velocity = horizontalVelocity + (Vector3.up * verticalVelocity);

                // apply the gravity to the velocity
                _velocity += gravityDownForce * dt * Vector3.down;
            }

            // Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
            // Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(_controller.height);
            _controller.Move(_velocity * dt);

            // m_LatestImpactSpeed = Vector3.zero;
            // if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, m_Controller.radius,
            //     CharacterVelocity.normalized, out RaycastHit hit, CharacterVelocity.magnitude * Time.deltaTime, -1,
            //     QueryTriggerInteraction.Ignore))
            // {
            //     m_LatestImpactSpeed = CharacterVelocity;

            //     CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hit.normal);
            // }

            // // move
            // Vector3 worldspaceMoveInput = transform.TransformVector(moveInput.Value.x, 0, moveInput.Value.y);
            // _controller.Move(dt * moveSpeed * worldspaceMoveInput);
        }

        void CheckImpact(bool wasGrounded)
        {
            // todo
        }

        void GroundCheck()
        {
            // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
            float chosenGroundCheckDistance =
                _isGrounded ? (_controller.skinWidth + groundCheckDistance) : k_GroundCheckDistanceInAir;

            // reset values before the ground check
            _isGrounded = false;
            _groundNormal = Vector3.up;

            // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
            if (Time.time >= _lastJumpTime + k_JumpGroundingPreventionTime)
            {
                // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
                if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(_controller.height),
                    _controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundCheckLayers,
                    QueryTriggerInteraction.Ignore))
                {
                    // storing the upward direction for the surface found
                    _groundNormal = hit.normal;

                    // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                    // and if the slope angle is lower than the character controller's limit
                    if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                        IsNormalUnderSlopeLimit(_groundNormal))
                    {
                        _isGrounded = true;

                        // handle snapping to the ground
                        if (hit.distance > _controller.skinWidth)
                        {
                            _controller.Move(Vector3.down * hit.distance);
                        }
                    }
                }
            }
        }

        bool IsNormalUnderSlopeLimit(Vector3 normal)
        {
            // fixes rounding errors, makes boundary between climbable/nonclimable feel better
            return (int)Vector3.Angle(transform.up, normal) <= (int)_controller.slopeLimit;
        }

        Vector3 GetCapsuleCenter()
        {
            return transform.position + _controller.center;
        }

        // returns center point of sphere overlapping bottom capsule hemisphere
        Vector3 GetCapsuleBottomHemisphere()
        {
            float halfHeight = Mathf.Max(_controller.height * 0.5f, _controller.radius);
            return GetCapsuleCenter() + Vector3.down * (halfHeight - _controller.radius);
        }

        Vector3 GetCapsuleTopHemisphere(float atHeight)
        {
            float halfHeight = Mathf.Max(atHeight * 0.5f, _controller.radius);
            return GetCapsuleCenter() + Vector3.up * (halfHeight - _controller.radius);
        }

        public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
        {
            Vector3 directionRight = Vector3.Cross(direction, transform.up);
            return Vector3.Cross(slopeNormal, directionRight).normalized;
        }

        public void SetSprintInput(bool inputDown) => _sprintInput = inputDown;
        public void SetJumpInput(bool inputDown) => _jumpInput = inputDown;
    }
}