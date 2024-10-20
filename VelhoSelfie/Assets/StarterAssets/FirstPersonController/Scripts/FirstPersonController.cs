using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 4.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 6.0f;

        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Tooltip("In degrees")]
        public float LookDownEventRotationThreshould = 30f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip(
            "Time required to pass before being able to jump again. Set to 0f to instantly jump again"
        )]
        public float JumpTimeout = 0.1f;

        [Tooltip(
            "Time required to pass before entering the fall state. Useful for walking down stairs"
        )]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip(
            "If the character is grounded or not. Not part of the CharacterController built in grounded check"
        )]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip(
            "The radius of the grounded check. Should match the radius of the CharacterController"
        )]
        public float GroundedRadius = 0.5f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip(
            "The follow target set in the Cinemachine Virtual Camera that the camera will follow"
        )]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        // cinemachine
        private float _cinemachineTargetPitch;
        private float _playerTargetYaw;

        // player
        private float _speed;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        Quaternion targetCamPitch; // up and down
        Quaternion targetCamYaw; // left and right

        [Range(0f, 1f)]
        [SerializeField]
        float rotationDamping = 0.1f;

        [SerializeField]
        float maxInteractiveDistance = 2f;
        readonly Vector3 interactionRaySource = new Vector3(0.5f, 0.5f, 0.0f);
        Ray InteractionRay => UnityEngine.Camera.main.ViewportPointToRay(interactionRaySource);
        bool interact;
        Interactive focusedObject;

        InputActions inputActions;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        public Animator animator;

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            inputActions = new InputActions();
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError(
                "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it"
            );
#endif

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            _playerTargetYaw = transform.localEulerAngles.y;
            _cinemachineTargetPitch = CinemachineCameraTarget.transform.localEulerAngles.x;
            CameraRotation();
        }

        private void OnEnable()
        {
            targetCamPitch = CinemachineCameraTarget.transform.localRotation;
            inputActions.Enable();
            GameEvents.CurrentView?.Set(View.FirstPerson);
        }

        private void OnDisable()
        {
            inputActions.Disable();
            focusedObject = null;
        }

        private void Update()
        {
            JumpAndGravity();
            GroundedCheck();
            Move();
            InteractionCheck();
        }

        private void FixedUpdate()
        {
            HandleInteraction();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(
                transform.position.x,
                transform.position.y - GroundedOffset,
                transform.position.z
            );
            Grounded = Physics.CheckSphere(
                spherePosition,
                GroundedRadius,
                GroundLayers,
                QueryTriggerInteraction.Ignore
            );
        }

        private void CameraRotation()
        {
            // if there is an input
            if (_input.look.sqrMagnitude >= _threshold)
            {
                // rotate the player left and right
                // don't multiply mouse input by Time.deltaTime
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                _playerTargetYaw += _input.look.x * RotationSpeed * deltaTimeMultiplier;
                targetCamYaw = Quaternion.Euler(0f, _playerTargetYaw, 0f);

                // rotate the player up and down
                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _cinemachineTargetPitch = ClampAngle(
                    _cinemachineTargetPitch,
                    BottomClamp,
                    TopClamp
                );
                targetCamPitch = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                GameEvents.PlayerIsLookingDown.Set(
                    _cinemachineTargetPitch > LookDownEventRotationThreshould
                );
            }

            CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(
                CinemachineCameraTarget.transform.localRotation,
                targetCamPitch,
                1f - rotationDamping
            );
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                targetCamYaw,
                1f - rotationDamping
            );
        }

        private void Move()
        {
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
            float targetSpeed = (_input.sprint ? SprintSpeed : MoveSpeed) * inputMagnitude;

            Vector3 targetVelocity =
                transform.right * _input.move.x + transform.forward * _input.move.y;
            targetVelocity.Normalize();
            targetVelocity *= targetSpeed;

            _controller.Move(
                (targetVelocity + new Vector3(0.0f, _verticalVelocity, 0.0f)) * Time.deltaTime
            );
            animator.SetFloat("Speed", Mathf.Abs(targetVelocity.magnitude));
            /*
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                // move
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            // move the player
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            */
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f)
                lfAngle += 360f;
            if (lfAngle > 360f)
                lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void InteractionCheck()
        {
            if (
                Cursor.lockState == CursorLockMode.Locked
                && inputActions.FirstPerson.Interact.triggered
            )
            {
                interact = true;
            }
        }

        private void HandleInteraction()
        {
            if (GameEvents.PlayerIsLookingDown)
                return;
            if (interact)
            {
                if (focusedObject != null && focusedObject.IsInteractive)
                {
                    focusedObject.Interact();
                    GameEvents.InteractionAttempted?.Raise(focusedObject);
                }
                else
                {
                    GameEvents.InteractionAttempted?.Raise(null);
                }
            }
            else
            {
                Interactive interactive = InteractionPhysics.GetHitInteractive(
                    InteractionRay,
                    maxInteractiveDistance
                );
                if (interactive == null)
                {
                    if (focusedObject != null)
                    {
                        if (focusedObject.IsFocused)
                        {
                            focusedObject.Unfocus();
                        }
                        focusedObject = null;
                    }
                }
                else if (interactive.IsInteractive)
                {
                    if (focusedObject == null)
                    {
                        focusedObject = interactive;
                        interactive.Focus();
                    }
                    else if (!System.Object.ReferenceEquals(focusedObject, interactive))
                    {
                        focusedObject.Unfocus();
                        focusedObject = interactive;
                        interactive.Focus();
                    }
                }
            }

            interact = false;
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded)
                Gizmos.color = transparentGreen;
            else
                Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(
                    transform.position.x,
                    transform.position.y - GroundedOffset,
                    transform.position.z
                ),
                GroundedRadius
            );
        }
    }
}
