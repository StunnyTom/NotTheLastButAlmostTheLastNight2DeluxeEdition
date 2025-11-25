using UnityEngine;

namespace MonsterSystem
{
    /// <summary>
    /// First-person controller for the Monster player
    /// Simplified from EasyPeasy, adapted for monster gameplay
    /// </summary>
    public class MonsterController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Range(0f, 20f)] public float walkSpeed = 4f;
        [Range(0f, 30f)] public float sprintSpeed = 7f;
        [Range(0f, 15f)] public float jumpSpeed = 3f;
        [Range(0f, 50f)] public float gravity = 9.81f;

        [Header("Look Settings")]
        [Range(0, 100)] public float mouseSensitivity = 50f;
        [Range(0f, 200f)] public float snappiness = 100f;

        [Header("Ground Check")]
        public Transform groundCheck;
        public float groundDistance = 0.2f;
        public LayerMask groundMask;

        [Header("Animation")]
        public Animator animator;

        [Header("Combat Settings")]
        public KeyCode attackKey = KeyCode.Mouse0; // Left mouse button
        public float attackCooldown = 1f; // Time between attacks
        private float lastAttackTime = -999f;

        [Header("Camera")]
        public Transform playerCamera;
        public Transform thirdPersonCamera; // for viewing animations
        public float thirdPersonDistance = 5f;
        public float thirdPersonHeight = 2f;
        private bool isFirstPerson = true; // Toggle between FP and TP

        // Components
        private CharacterController characterController;
        private Camera cam;

        // Movement
        private Vector3 moveDirection = Vector3.zero;
        private Vector2 moveInput;
        private bool isGrounded;
        private bool isSprinting;
        private bool isJumping;

        // Look
        private float rotX, rotY;
        private float xVelocity, yVelocity;

        // Control flags
        private bool isLookEnabled = true;
        private bool isMoveEnabled = true;

        void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            cam = playerCamera.GetComponent<Camera>();

            // Debug
            if (animator == null)
            {
                Debug.LogError("Animator component not found on monster!");
            }
            else
            {
                Debug.Log("Animator found successfully!");
            }
            // Lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Initialize rotation
            rotX = transform.rotation.eulerAngles.y;
            rotY = playerCamera.localRotation.eulerAngles.x;
            xVelocity = rotX;
            yVelocity = rotY;
        }

        void Update()
        {
            CheckGrounded();
            HandleLook();
            HandleMovement();
            HandleAttack();
            UpdateAnimations();
            HandleCameraToggle();
        }

        void UpdateAnimations()
        {
            if (animator == null) return;

            // Calculate movement magnitude (0 to 1)
            float moveMagnitude = new Vector2(moveInput.x, moveInput.y).magnitude;

            // Determine speed for animator
            float animSpeed = 0f;

            if (moveMagnitude > 0.1f)
            {
                if (isSprinting)
                {
                    animSpeed = 1f; // Running
                }
                else
                {
                    animSpeed = 0.5f; // Walking
                }
            }
            else
            {
                animSpeed = 0f; // Idle
            }

            // Smooth the speed transition
            float currentSpeed = animator.GetFloat("Speed");
            float smoothSpeed = Mathf.Lerp(currentSpeed, animSpeed, Time.deltaTime * 10f);

            // Set animator parameters
            animator.SetFloat("Speed", smoothSpeed);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsJumping", isJumping);
        }

        void CheckGrounded()
        {
            bool wasGrounded = isGrounded;
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            // Detect landing
            if (!wasGrounded && isGrounded)
            {
                isJumping = false;
            }

            if (isGrounded && moveDirection.y < 0)
            {
                moveDirection.y = -2f;
            }
        }

        void HandleLook()
        {
            if (!isLookEnabled) return;

            float mouseX = Input.GetAxis("Mouse X") * 10 * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * 10 * mouseSensitivity * Time.deltaTime;

            rotX += mouseX;
            rotY -= mouseY;
            rotY = Mathf.Clamp(rotY, -90f, 90f);

            // Smooth rotation
            xVelocity = Mathf.Lerp(xVelocity, rotX, snappiness * Time.deltaTime);
            yVelocity = Mathf.Lerp(yVelocity, rotY, snappiness * Time.deltaTime);

            // Apply rotation
            playerCamera.transform.localRotation = Quaternion.Euler(yVelocity, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, xVelocity, 0f);
        }

        void HandleMovement()
        {
            if (!isMoveEnabled) return;

            // Get input
            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.y = Input.GetAxis("Vertical");

            // Sprint
            isSprinting = Input.GetKey(KeyCode.LeftShift) && moveInput.y > 0.1f && isGrounded;
            float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

            // Calculate movement direction (relative to character rotation)
            Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
            direction = Vector3.ClampMagnitude(direction, 1f); // Prevent faster diagonal movement
            Vector3 moveVector = transform.TransformDirection(direction) * currentSpeed;

            // Handle vertical movement (gravity/jumping)
            if (isGrounded)
            {
                // Reset vertical velocity when on ground
                moveDirection.y = -2f;

                // Jump
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    moveDirection.y = jumpSpeed;
                    isJumping = true;
                    
                    // Trigger jump animation
                    if (animator != null)
                    {
                        animator.SetTrigger("Jump");
                    }
                }
            }
            else
            {
                // Apply gravity when in air
                moveDirection.y -= gravity * Time.deltaTime;
            }

            // Combine horizontal movement with vertical movement
            moveDirection.x = moveVector.x;
            moveDirection.z = moveVector.z;

            // Move the character controller
            characterController.Move(moveDirection * Time.deltaTime);
        }

        void HandleAttack()
        {
            // Check if attack button pressed and cooldown passed
            if (Input.GetKeyDown(attackKey))
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    PerformAttack();
                    lastAttackTime = Time.time;
                }
            }
        }

        void PerformAttack()
        {
            if (animator == null) return;

            // Trigger attack animation
            animator.SetTrigger("Attack");

            Debug.Log("Monster attacked!");

            // TODO: Add attack logic here (damage detection, collision, etc.)
            // For example:
            // - Raycast forward to detect survivors
            // - Apply damage if hit
            // - Play attack sound effect
        }

        void HandleCameraToggle()
        {
            // Press C to toggle camera
            if (Input.GetKeyDown(KeyCode.C))
            {
                isFirstPerson = !isFirstPerson;

                if (isFirstPerson)
                {
                    // First person camera
                    playerCamera.gameObject.SetActive(true);
                    if (thirdPersonCamera != null)
                        thirdPersonCamera.gameObject.SetActive(false);
                }
                else
                {
                    // Third person camera
                    playerCamera.gameObject.SetActive(false);
                    if (thirdPersonCamera != null)
                    {
                        thirdPersonCamera.gameObject.SetActive(true);
                        // Position third person camera behind and above
                        Vector3 offset = -transform.forward * thirdPersonDistance + Vector3.up * thirdPersonHeight;
                        thirdPersonCamera.position = transform.position + offset;
                        thirdPersonCamera.LookAt(transform.position + Vector3.up * 1.5f);
                    }
                }
            }

            // Update third person camera position if active
            if (!isFirstPerson && thirdPersonCamera != null)
            {
                Vector3 targetPosition = transform.position - transform.forward * thirdPersonDistance + Vector3.up * thirdPersonHeight;
                thirdPersonCamera.position = Vector3.Lerp(thirdPersonCamera.position, targetPosition, Time.deltaTime * 5f);
                thirdPersonCamera.LookAt(transform.position + Vector3.up * 1.5f);
            }
        }

        #region Public Control Methods
        /// <summary>
        /// Enable/disable both look and move control
        /// </summary>
        public void SetControl(bool newState)
        {
            SetLookControl(newState);
            SetMoveControl(newState);
        }

        /// <summary>
        /// Enable/disable camera look control
        /// </summary>
        public void SetLookControl(bool newState)
        {
            isLookEnabled = newState;
        }

        /// <summary>
        /// Enable/disable movement control
        /// </summary>
        public void SetMoveControl(bool newState)
        {
            isMoveEnabled = newState;
        }

        /// <summary>
        /// Show/hide cursor
        /// </summary>
        public void SetCursorVisibility(bool visible)
        {
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = visible;
        }

        /// <summary>
        /// Check if monster is currently sprinting
        /// </summary>
        public bool IsSprinting => isSprinting;

        /// <summary>
        /// Check if monster is currently jumping
        /// </summary>
        public bool IsJumping => isJumping;

        /// <summary>
        /// Get current movement input (for animations)
        /// </summary>
        public Vector2 GetMoveInput => moveInput;

        /// <summary>
        /// Manually trigger an attack (for multiplayer/external systems)
        /// </summary>
        public void TriggerAttack()
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                PerformAttack();
                lastAttackTime = Time.time;
            }
        }
        #endregion
    }
}