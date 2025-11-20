using UnityEngine;


namespace PlayerController // Or any other appropriate namespace
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 3f;
        [SerializeField] private float runSpeed = 6f;
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float gravity = -9.81f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private LayerMask groundMask = 1;

        [Header("Movement Smoothing")]
        [SerializeField] private float accelerationTime = 0.1f;
        [SerializeField] private float decelerationTime = 0.1f;

        [Header("Input Settings")]
        [SerializeField] private KeyCode runKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;

        // Components
        private CharacterController controller;

        // Movement variables
        private Vector3 velocity;
        private bool isGrounded;
        private Vector2 currentInputVector;
        private Vector2 smoothInputVelocity;

        // Movement state
        private bool isRunning;
        private float currentSpeed;

        void Start()
        {
            // Get required components
            controller = GetComponent<CharacterController>();

            // Create ground check if it doesn't exist
            if (groundCheck == null)
            {
                GameObject groundCheckObj = new GameObject("GroundCheck");
                groundCheckObj.transform.SetParent(transform);
                groundCheckObj.transform.localPosition = new Vector3(0, -controller.height / 2, 0);
                groundCheck = groundCheckObj.transform;
            }
        }

        void Update()
        {
            HandleGroundCheck();
            HandleInput();
            HandleMovement();
            HandleGravityAndJump();

            // Apply movement to character controller
            controller.Move(velocity * Time.deltaTime);
        }

        private void HandleGroundCheck()
        {
            // Check if player is grounded
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            // Reset velocity when grounded
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Small negative value to keep grounded
            }
        }

        private void HandleInput()
        {
            // Get input
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // Check if running
            isRunning = Input.GetKey(runKey);

            // Create input vector
            Vector2 targetInputVector = new Vector2(horizontal, vertical).normalized;

            // Smooth input for better movement feel
            float smoothTime = targetInputVector.magnitude > 0 ? accelerationTime : decelerationTime;
            currentInputVector = Vector2.SmoothDamp(currentInputVector, targetInputVector, ref smoothInputVelocity, smoothTime);
        }

        private void HandleMovement()
        {
            // Calculate current speed based on running state
            currentSpeed = isRunning ? runSpeed : walkSpeed;

            // Calculate movement direction relative to player rotation
            Vector3 moveDirection = transform.right * currentInputVector.x + transform.forward * currentInputVector.y;

            // Apply movement
            velocity.x = moveDirection.x * currentSpeed;
            velocity.z = moveDirection.z * currentSpeed;
        }

        private void HandleGravityAndJump()
        {
            // Handle jumping
            if (Input.GetKeyDown(jumpKey) && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
        }

        // Public methods for external access
        public bool IsGrounded()
        {
            return isGrounded;
        }

        public bool IsRunning()
        {
            return isRunning && currentInputVector.magnitude > 0.1f;
        }

        public bool IsMoving()
        {
            return currentInputVector.magnitude > 0.1f;
        }

        public float GetCurrentSpeed()
        {
            return currentSpeed;
        }

        public Vector3 GetVelocity()
        {
            return velocity;
        }

        public void SetMovementSpeeds(float newWalkSpeed, float newRunSpeed)
        {
            walkSpeed = newWalkSpeed;
            runSpeed = newRunSpeed;
        }

        public void SetJumpHeight(float newJumpHeight)
        {
            jumpHeight = newJumpHeight;
        }

        // Gizmos for debugging
        private void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = isGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
            }
        }
    }
}