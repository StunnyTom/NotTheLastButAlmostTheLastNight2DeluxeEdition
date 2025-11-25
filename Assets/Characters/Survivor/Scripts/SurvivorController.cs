using UnityEngine;

namespace SurvivorSystem
{
    /// <summary>
    /// Controller for Survivor players
    /// Based on MonsterController but without attack
    /// </summary>
    public class SurvivorController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Range(0f, 20f)] public float walkSpeed = 3f;
        [Range(0f, 30f)] public float sprintSpeed = 5f;
        [Range(0f, 20f)] public float crouchSpeed = 1.5f;
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

        [Header("Camera")]
        public Transform playerCamera;
        public Transform thirdPersonCamera;
        public float thirdPersonDistance = 5f;
        public float thirdPersonHeight = 2f;
        private bool isFirstPerson = true;

        [Header("First Person View")]
        public SkinnedMeshRenderer survivorMeshRenderer;
        public bool hideBodyInFirstPerson = true;

        // Components
        private CharacterController characterController;
        private Camera cam;

        // Movement
        private Vector3 moveDirection = Vector3.zero;
        private Vector2 moveInput;
        private bool isGrounded;
        private bool isSprinting;
        private bool isCrouching;
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

            if (playerCamera != null)
                cam = playerCamera.GetComponent<Camera>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            rotX = transform.rotation.eulerAngles.y;
            rotY = playerCamera != null ? playerCamera.localRotation.eulerAngles.x : 0;
            xVelocity = rotX;
            yVelocity = rotY;
        }

        void Update()
        {
            CheckGrounded();
            HandleLook();
            HandleMovement();
            UpdateAnimations();
            HandleCameraToggle();
        }

        void UpdateAnimations()
        {
            if (animator == null) return;

            float moveMagnitude = new Vector2(moveInput.x, moveInput.y).magnitude;
            float animSpeed = 0f;

            if (moveMagnitude > 0.1f)
            {
                if (isSprinting)
                    animSpeed = 1f; // Running
                else if (isCrouching)
                    animSpeed = 0.3f; // Crouch walk
                else
                    animSpeed = 0.5f; // Walking
            }

            float currentSpeed = animator.GetFloat("Speed");
            float smoothSpeed = Mathf.Lerp(currentSpeed, animSpeed, Time.deltaTime * 10f);

            animator.SetFloat("Speed", smoothSpeed);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsJumping", isJumping);
            animator.SetBool("IsCrouching", isCrouching);
        }

        void CheckGrounded()
        {
            bool wasGrounded = isGrounded;
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (!wasGrounded && isGrounded)
                isJumping = false;

            if (isGrounded && moveDirection.y < 0)
                moveDirection.y = -2f;
        }

        void HandleLook()
        {
            if (!isLookEnabled) return;

            float mouseX = Input.GetAxis("Mouse X") * 10 * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * 10 * mouseSensitivity * Time.deltaTime;

            rotX += mouseX;
            rotY -= mouseY;
            rotY = Mathf.Clamp(rotY, -90f, 90f);

            xVelocity = Mathf.Lerp(xVelocity, rotX, snappiness * Time.deltaTime);
            yVelocity = Mathf.Lerp(yVelocity, rotY, snappiness * Time.deltaTime);

            if (playerCamera != null)
            {
                playerCamera.transform.localRotation = Quaternion.Euler(yVelocity, 0f, 0f);
                transform.rotation = Quaternion.Euler(0f, xVelocity, 0f);
            }
        }

        void HandleMovement()
        {
            if (!isMoveEnabled) return;

            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.y = Input.GetAxis("Vertical");

            // Crouch
            isCrouching = Input.GetKey(KeyCode.LeftControl);

            // Sprint (can't sprint while crouching)
            isSprinting = !isCrouching && Input.GetKey(KeyCode.LeftShift) && moveInput.y > 0.1f && isGrounded;

            float currentSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);

            Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
            direction = Vector3.ClampMagnitude(direction, 1f);
            Vector3 moveVector = transform.TransformDirection(direction) * currentSpeed;

            if (isGrounded)
            {
                moveDirection.y = -2f;

                if (Input.GetKeyDown(KeyCode.Space) && !isCrouching)
                {
                    moveDirection.y = jumpSpeed;
                    isJumping = true;

                    if (animator != null)
                        animator.SetTrigger("Jump");
                }
            }
            else
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            moveDirection.x = moveVector.x;
            moveDirection.z = moveVector.z;

            characterController.Move(moveDirection * Time.deltaTime);
        }

        void HandleCameraToggle()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                isFirstPerson = !isFirstPerson;

                if (isFirstPerson)
                {
                    if (playerCamera != null)
                        playerCamera.gameObject.SetActive(true);
                    if (thirdPersonCamera != null)
                        thirdPersonCamera.gameObject.SetActive(false);

                    if (hideBodyInFirstPerson && survivorMeshRenderer != null)
                        survivorMeshRenderer.enabled = false;
                }
                else
                {
                    if (playerCamera != null)
                        playerCamera.gameObject.SetActive(false);
                    if (thirdPersonCamera != null)
                    {
                        thirdPersonCamera.gameObject.SetActive(true);
                        Vector3 offset = -transform.forward * thirdPersonDistance + Vector3.up * thirdPersonHeight;
                        thirdPersonCamera.position = transform.position + offset;
                        thirdPersonCamera.LookAt(transform.position + Vector3.up * 1.5f);
                    }

                    if (survivorMeshRenderer != null)
                        survivorMeshRenderer.enabled = true;
                }
            }

            if (!isFirstPerson && thirdPersonCamera != null)
            {
                Vector3 targetPosition = transform.position - transform.forward * thirdPersonDistance + Vector3.up * thirdPersonHeight;
                thirdPersonCamera.position = Vector3.Lerp(thirdPersonCamera.position, targetPosition, Time.deltaTime * 5f);
                thirdPersonCamera.LookAt(transform.position + Vector3.up * 1.5f);
            }
        }

        public void SetControl(bool newState)
        {
            isLookEnabled = newState;
            isMoveEnabled = newState;
        }

        public bool IsSprinting => isSprinting;
        public bool IsCrouching => isCrouching;
        public Vector2 GetMoveInput => moveInput;
    }
}