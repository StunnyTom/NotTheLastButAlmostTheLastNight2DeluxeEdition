using UnityEngine;
using System.Collections;

namespace MonsterSystem
{
    public class MonsterController : MonoBehaviour
    {
        [Header("Chase Phase Settings")]
        public KeyCode triggerChaseKey = KeyCode.R;
        [Tooltip("How long the monster is frozen/roaring before moving.")]
        public float chaseWarmupDuration = 5.0f;
        [Tooltip("How long the actual chase lasts after the warmup.")]
        public float chaseDuration = 15.0f;
        [Tooltip("If true, the mesh is disabled on Start until Chase begins.")]
        public bool startInvisible = true;

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
        public KeyCode attackKey = KeyCode.Mouse0;
        public float attackCooldown = 1f;
        private float lastAttackTime = -999f;

        [Header("Camera")]
        public Transform playerCamera;
        public Transform thirdPersonCamera;
        public float thirdPersonDistance = 5f;
        public float thirdPersonHeight = 2f;
        private bool isFirstPerson = true;

        [Header("First Person View")]
        public SkinnedMeshRenderer monsterMeshRenderer;
        public bool hideBodyInFirstPerson = true;

        // Components
        private CharacterController characterController;
        private Camera cam;

        // Movement States
        private Vector3 moveDirection = Vector3.zero;
        private Vector2 moveInput;
        private bool isGrounded;
        private bool isSprinting;
        private bool isJumping;

        // Look States
        private float rotX, rotY;
        private float xVelocity, yVelocity;

        // Control flags
        private bool isLookEnabled = true;
        private bool isMoveEnabled = true;

        // Chase Flags
        private bool isStealthMode = false;
        private bool isChasing = false; // Replaced hasChaseStarted to allow resetting

        void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            cam = playerCamera.GetComponent<Camera>();

            if (animator == null) Debug.LogError("Animator component not found on monster!");

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            rotX = transform.rotation.eulerAngles.y;
            rotY = playerCamera.localRotation.eulerAngles.x;
            xVelocity = rotX;
            yVelocity = rotY;
        }

        void Start()
        {
            // Initialize Stealth Logic
            if (startInvisible)
            {
                SetStealthState(true);
            }
        }

        void Update()
        {
            CheckGrounded();
            HandleLook();
            HandleMovement();
            HandleAttack();
            UpdateAnimations();
            HandleCameraToggle();

            // New Chase Logic
            HandleChaseTrigger();
        }

        // ---------------------------------------------------------
        // UPDATED CHASE LOGIC
        // ---------------------------------------------------------

        void HandleChaseTrigger()
        {
            // Only trigger if we aren't already in a chase sequence
            if (Input.GetKeyDown(triggerChaseKey) && !isChasing)
            {
                StartCoroutine(ChasePhaseRoutine());
            }
        }

        IEnumerator ChasePhaseRoutine()
        {
            isChasing = true;
            Debug.Log("PHASE 1: REVEAL & WARMUP");

            // 1. Reveal (Visible)
            SetStealthState(false);

            // 2. Freeze Movement
            isMoveEnabled = false;

            // 3. Play Roar
            if (animator != null)
            {
                animator.SetTrigger("Roar");
            }

            // 4. Wait for Warmup (e.g., 5 seconds)
            yield return new WaitForSeconds(chaseWarmupDuration);

            // -----------------------------------------------------

            Debug.Log("PHASE 2: CHASE ACTIVE");

            // 5. Unfreeze Movement
            isMoveEnabled = true;

            // 6. Wait for Chase Duration (e.g., 15 seconds)
            // You can add logic here to play "Chase Music" if you have an AudioSource
            yield return new WaitForSeconds(chaseDuration);

            // -----------------------------------------------------

            Debug.Log("PHASE 3: CHASE ENDED");
            EndChase();
        }

        void EndChase()
        {
            // Return to Stealth
            SetStealthState(true);

            // Allow triggering the chase again
            isChasing = false;
        }

        public void SetStealthState(bool isStealth)
        {
            isStealthMode = isStealth;

            // Toggle Renderer visibility
            if (monsterMeshRenderer != null)
            {
                monsterMeshRenderer.enabled = !isStealth;
            }
        }

        // ---------------------------------------------------------
        // EXISTING LOGIC
        // ---------------------------------------------------------

        void UpdateAnimations()
        {
            if (animator == null) return;

            float moveMagnitude = new Vector2(moveInput.x, moveInput.y).magnitude;
            float animSpeed = 0f;

            if (moveMagnitude > 0.1f)
                animSpeed = isSprinting ? 1f : 0.5f;
            else
                animSpeed = 0f;

            float currentSpeed = animator.GetFloat("Speed");
            float smoothSpeed = Mathf.Lerp(currentSpeed, animSpeed, Time.deltaTime * 10f);

            animator.SetFloat("Speed", smoothSpeed);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsJumping", isJumping);
        }

        void CheckGrounded()
        {
            bool wasGrounded = isGrounded;
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (!wasGrounded && isGrounded) isJumping = false;
            if (isGrounded && moveDirection.y < 0) moveDirection.y = -2f;
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

            playerCamera.transform.localRotation = Quaternion.Euler(yVelocity, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, xVelocity, 0f);
        }

        void HandleMovement()
        {
            if (!isMoveEnabled)
            {
                moveInput = Vector2.zero;
                // Apply gravity even when frozen
                if (!isGrounded) moveDirection.y -= gravity * Time.deltaTime;
                characterController.Move(new Vector3(0, moveDirection.y, 0) * Time.deltaTime);
                return;
            }

            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.y = Input.GetAxis("Vertical");

            isSprinting = Input.GetKey(KeyCode.LeftShift) && moveInput.y > 0.1f && isGrounded;
            float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

            Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
            direction = Vector3.ClampMagnitude(direction, 1f);
            Vector3 moveVector = transform.TransformDirection(direction) * currentSpeed;

            if (isGrounded)
            {
                moveDirection.y = -2f;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    moveDirection.y = jumpSpeed;
                    isJumping = true;
                    if (animator != null) animator.SetTrigger("Jump");
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

        void HandleAttack()
        {
            if (!isMoveEnabled) return;

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
            animator.SetTrigger("Attack");
            Debug.Log("Monster attacked!");
        }

        void HandleCameraToggle()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                isFirstPerson = !isFirstPerson;

                if (isFirstPerson)
                {
                    if (hideBodyInFirstPerson && monsterMeshRenderer != null)
                    {
                        monsterMeshRenderer.enabled = false;
                    }
                    playerCamera.gameObject.SetActive(true);
                    if (thirdPersonCamera != null) thirdPersonCamera.gameObject.SetActive(false);
                }
                else
                {
                    playerCamera.gameObject.SetActive(false);
                    if (thirdPersonCamera != null)
                    {
                        thirdPersonCamera.gameObject.SetActive(true);
                        Vector3 offset = -transform.forward * thirdPersonDistance + Vector3.up * thirdPersonHeight;
                        thirdPersonCamera.position = transform.position + offset;
                        thirdPersonCamera.LookAt(transform.position + Vector3.up * 1.5f);
                    }

                    // Visibility logic for 3rd person
                    if (monsterMeshRenderer != null)
                    {
                        // Only show body if we are NOT in stealth mode
                        monsterMeshRenderer.enabled = !isStealthMode;
                    }
                }
            }

            if (!isFirstPerson && thirdPersonCamera != null)
            {
                Vector3 targetPosition = transform.position - transform.forward * thirdPersonDistance + Vector3.up * thirdPersonHeight;
                thirdPersonCamera.position = Vector3.Lerp(thirdPersonCamera.position, targetPosition, Time.deltaTime * 5f);
                thirdPersonCamera.LookAt(transform.position + Vector3.up * 1.5f);
            }
        }
    }
}