using Potato.Game;
using Potato.Core;
using UnityEngine;

namespace Potato.Gameplay
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerWeaponsManager), typeof(FirstPersonAnimationController))]
    public class PlayerCharacterController : MonoBehaviour
    {
        const float k_JumpGroundingPreventionTime = 0.2f;
        const float k_GroundCheckDistanceInAir = 0.07f;

        [Header("Main Camera")]
        [SerializeField] private Camera playerCamera;

        [Header("Input Data")]
        [SerializeField] private InputFloatAxis moveInput;
        [SerializeField] private InputFloatAxis lookInput;
        [SerializeField] private InputButton sprintInput;
        [SerializeField] private InputButton crouchInput;

        [Header("Settings")]
        [SerializeField] private PlayerCharacterControllerReference playerRef;
        [SerializeField] private BoolReference isPausedRef;

        [Header("Gravity")]
        [SerializeField] private float gravityDownForce = 20f;
        [SerializeField] private LayerMask groundCheckLayers;
        [SerializeField] private float groundCheckDistance = 0.05f;

        [Header("Stance")]
        [SerializeField] private float crouchingSharpness = 10f;
        [SerializeField] private float crouchingHeight = .9f;
        [SerializeField] private float standingHeight = 1.8f;
        [SerializeField] private float cameraHeightRatio = .9f;
        [SerializeField][Range(0f, 1f)] private float crouchSpeedModifier = .5f;

        [Header("Movement")]
        [SerializeField] private float sprintSpeedModifier = 1.5f;
        [SerializeField] private float maxGroundSpeed = 13f;
        [SerializeField] private float groundTurningSharpness = 15f;
        [SerializeField] private float maxAirSpeed = 10f;
        [SerializeField] private float airAcceleration = 25f;
        [SerializeField] private float turnSpeed = 1000f;
        [SerializeField] private float jumpForce = 9f;

        [Header("Ai Detection")]
        [SerializeField] private Transform[] detectionPoints;

        // --
        private CharacterController _controller;
        private FirstPersonAnimationController _animationController;
        private PlayerWeaponsManager _weapons;
        private Vector3 _velocity = Vector3.zero;
        private float _cameraY = 0;
        private bool _isGrounded = false;
        private bool _isCrouching = false;
        private bool _isSprinting = false;
        private Vector3 _groundNormal = Vector3.up;
        private float _lastJumpTime = 0f;
        private float _targetCharacterHeight;
        private Collider[] _crouchOverlapBuffer = new Collider[4];

        // --
        public bool IsAlive => true;
        public bool IsGrounded => _isGrounded;
        public float MaxSpeedOnGround => maxGroundSpeed;
        public float SprintSpeedModifier => sprintSpeedModifier;
        public Transform[] DetectionPoints => detectionPoints;

        void Start()
        {
            _controller = GetComponent<CharacterController>();
            _animationController = GetComponent<FirstPersonAnimationController>();
            _weapons = GetComponent<PlayerWeaponsManager>();
            _controller.enableOverlapRecovery = true;

            SetCrouchingState(false, true);
            UpdateCharacterHeight(true);
        }

        void OnEnable()
        {
            playerRef.Value = this;
        }

        void OnDisable()
        {
            playerRef.Value = null;
        }

        void Update()
        {
            if (!isPausedRef.Value)
            {
                bool wasGrounded = _isGrounded;
                GroundCheck();
                CheckFallDamage(wasGrounded);
                UpdateCrouchInput();
                UpdateCharacterHeight(false);
                UpdateCamera();
                UpdateMovement(Time.deltaTime);
            }
        }

        void LateUpdate()
        {
            _animationController.LateUpdateWeaponBob(transform.position, _isGrounded, _weapons.IsAiming, MaxSpeedOnGround, SprintSpeedModifier);
        }

        void UpdateCamera()
        {
            // camera x
            transform.Rotate(0f, lookInput.Value.x * turnSpeed, 0f);

            // camera y
            _cameraY += lookInput.Value.y * turnSpeed;
            _cameraY = Mathf.Clamp(_cameraY, -89f, 89f);
            playerCamera.transform.localEulerAngles = new Vector3(_cameraY, 0, 0);
        }

        void UpdateMovement(float dt)
        {
            // sprinting
            _isSprinting = sprintInput.ButtonDown;
            if (_isSprinting)
                _isSprinting = SetCrouchingState(false, false);

            float speedModifier = _isSprinting ? sprintSpeedModifier : 1f;
            Vector3 worldspaceMoveInput = transform.TransformVector(moveInput.Value.x, 0f, moveInput.Value.y);

            // normalize diagonal speed (wouldn't work if controllers were supported)
            if (worldspaceMoveInput.x != 0 && worldspaceMoveInput.z != 0)
                worldspaceMoveInput.Normalize();

            // movement
            if (_isGrounded)
            {
                Vector3 targetVelocity = maxGroundSpeed * speedModifier * worldspaceMoveInput;

                if (_isCrouching)
                    targetVelocity *= crouchSpeedModifier;
                targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, _groundNormal) *
                                 targetVelocity.magnitude;

                _velocity = Vector3.Lerp(_velocity, targetVelocity, groundTurningSharpness * dt);
                _animationController.UpdateFootstepSfx(_velocity.magnitude * dt, _isSprinting);
            }
            // air movement
            else
            {
                // add air acceleration
                _velocity += airAcceleration * dt * worldspaceMoveInput;

                // limit air speed to a maximum, but only horizontally
                float verticalVelocity = _velocity.y;
                Vector3 horizontalVelocity = Vector3.ProjectOnPlane(_velocity, Vector3.up);
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxAirSpeed * speedModifier);
                _velocity = horizontalVelocity + (Vector3.up * verticalVelocity);

                // apply the gravity to the velocity
                _velocity += gravityDownForce * dt * Vector3.down;
            }

            Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
            Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(_controller.height);

            // apply movement
            _controller.Move(_velocity * dt);

            // ground impact
            if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, _controller.radius,
                _velocity.normalized, out RaycastHit hit, _velocity.magnitude * dt, groundCheckLayers,
                QueryTriggerInteraction.Ignore))
            {

                _velocity = Vector3.ProjectOnPlane(_velocity, hit.normal);
            }
        }

        public void TryJumping()
        {
            if (_isGrounded)
            {
                // force the crouch state to false
                if (SetCrouchingState(false, false))
                {
                    _velocity = new Vector3(_velocity.x, 0f, _velocity.z);
                    _velocity += Vector3.up * jumpForce;

                    _animationController.PlaySfx_Jump();

                    // remember last time we jumped because we need to prevent snapping to ground for a short time
                    _lastJumpTime = Time.time;

                    // Force grounding to false
                    _isGrounded = false;
                    _groundNormal = Vector3.up;
                }
            }
        }

        void CheckFallDamage(bool wasGrounded)
        {
            if (_isGrounded && !wasGrounded)
            {
                // // Fall damage
                // float fallSpeed = -Mathf.Min(_velocity.y, _lastImpactSpeed.y);
                // float fallSpeedRatio = (fallSpeed - MinSpeedForFallDamage) /
                //                        (MaxSpeedForFallDamage - MinSpeedForFallDamage);
                // if (receivesFallDamage && fallSpeedRatio > 0f)
                // {
                //     float dmgFromFall = Mathf.Lerp(FallDamageAtMinSpeed, FallDamageAtMaxSpeed, fallSpeedRatio);
                //     // m_Health.TakeDamage(dmgFromFall, null);

                //     // // fall damage SFX
                //     // AudioSource.PlayOneShot(FallDamageSfx);
                // }
                // else
                // {
                //     // land SFX
                //     //AudioSource.PlayOneShot(LandSfx);
                // }
                _animationController.PlaySfx_Land();
            }
        }

        void UpdateCharacterHeight(bool force)
        {
            // Update height instantly
            if (force)
            {
                _controller.height = _targetCharacterHeight;
                _controller.center = _controller.height * 0.5f * Vector3.up;
                playerCamera.transform.localPosition = _targetCharacterHeight * cameraHeightRatio * Vector3.up;
            }
            // Update smooth height
            else if (_controller.height != _targetCharacterHeight)
            {
                // resize the capsule and adjust camera position
                _controller.height = Mathf.Lerp(_controller.height, _targetCharacterHeight,
                    crouchingSharpness * Time.deltaTime);
                _controller.center = _controller.height * 0.5f * Vector3.up;
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition,
                    _targetCharacterHeight * cameraHeightRatio * Vector3.up, crouchingSharpness * Time.deltaTime);
            }
        }

        void UpdateCrouchInput()
        {
            // crouch state doesn't need updating
            if (crouchInput.ButtonDown == _isCrouching)
                return;

            var wantsCrouch = crouchInput.ButtonDown && !_isSprinting;
            SetCrouchingState(wantsCrouch, false);
        }

        // returns false if there was an obstruction
        bool SetCrouchingState(bool crouched, bool ignoreObstructions)
        {
            // set appropriate heights
            if (crouched)
            {
                _targetCharacterHeight = crouchingHeight;
            }
            else
            {
                // Detect obstructions
                if (!ignoreObstructions)
                {
                    int overlapCount = Physics.OverlapCapsuleNonAlloc(
                        GetCapsuleBottomHemisphere(),
                        GetCapsuleTopHemisphere(standingHeight),
                        _controller.radius,
                        _crouchOverlapBuffer,
                        groundCheckLayers,
                        QueryTriggerInteraction.Ignore);

                    for (int i = 0; i < overlapCount; ++i)
                    {
                        if (_crouchOverlapBuffer[i] != _controller)
                            return false;
                    }
                }

                _targetCharacterHeight = standingHeight;
            }

            _isCrouching = crouched;
            return true;
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
    }
}