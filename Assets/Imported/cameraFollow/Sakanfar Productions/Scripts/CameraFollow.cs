using UnityEngine;


namespace FollowCamera // Or any other appropriate namespace
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Mouse Look Settings")]
        [SerializeField] private float mouseSensitivityX = 100f;
        [SerializeField] private float mouseSensitivityY = 100f;
        [SerializeField] private bool invertMouseY = false;

        [Header("Camera Constraints")]
        [SerializeField] private float minVerticalAngle = -90f;
        [SerializeField] private float maxVerticalAngle = 90f;

        [Header("Smoothing")]
        [SerializeField] private float smoothTime = 0.1f;
        [SerializeField] private bool enableSmoothing = true;        [Header("References")]
        [SerializeField] private Transform playerBody;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private PlayerController.PlayerController playerController;[Header("Zoom/Aim Settings")]
        [SerializeField] private KeyCode aimKey = KeyCode.Mouse1;
        [SerializeField] private float normalFOV = 60f;
        [SerializeField] private float zoomedFOV = 30f;
        [SerializeField] private float runningFOV = 70f;
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float aimSensitivityMultiplier = 0.5f;

        [Header("Camera Shake Settings")]
        [SerializeField] private bool enableCameraShake = true;
        [SerializeField] private float walkShakeIntensity = 0.02f;
        [SerializeField] private float runShakeIntensity = 0.05f;
        [SerializeField] private float shakeFrequency = 10f;
        [SerializeField] private float landingShakeIntensity = 0.15f;
        [SerializeField] private float landingShakeDuration = 0.3f;
        [SerializeField] private float shakeReduction = 2f;

        // Private variables for mouse look
        private float xRotation = 0f;
        private float yRotation = 0f;
        private Vector2 currentMouseDelta;
        private Vector2 currentMouseDeltaVelocity;
        // Zoom variables
        private bool isAiming = false;
        private float targetFOV;
        private float currentFOV;

        // Camera shake variables
        private Vector3 cameraShakeOffset;
        private float shakeTimer;
        private float landingShakeTimer;
        private bool wasGrounded = true;
        void Start()
        {
            // Lock cursor to center of screen and hide it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Auto-assign references if not set
            if (playerCamera == null)
                playerCamera = GetComponent<Camera>();            // Try to find player body if not assigned
            if (playerBody == null)
            {
                // First try parent
                if (transform.parent != null)
                    playerBody = transform.parent;
                else
                {
                    // Try to find PlayerController in scene
                    PlayerController.PlayerController playerControllerComponent = FindFirstObjectByType<PlayerController.PlayerController>();
                    if (playerControllerComponent != null)
                        playerBody = playerControllerComponent.transform;
                }
            }

            // Try to find PlayerController if not assigned
            if (playerController == null)
            {
                if (playerBody != null)
                    playerController = playerBody.GetComponent<PlayerController.PlayerController>();
                else
                    playerController = FindFirstObjectByType<PlayerController.PlayerController>();
            }
            // Initialize rotation values
            if (playerBody != null)
            {
                yRotation = playerBody.eulerAngles.y;
                Debug.Log("CameraFollow: Player Body found and assigned: " + playerBody.name);
            }
            else
            {
                Debug.LogError("CameraFollow: No Player Body found! Please assign the player body in the inspector or make sure the camera is a child of the player.");
            }

            // Initialize zoom values
            if (playerCamera != null)
            {
                normalFOV = playerCamera.fieldOfView;
                currentFOV = normalFOV;
                targetFOV = normalFOV;
            }
        }
        void Update()
        {
            HandleMouseLook();
            HandleZoom();
            HandleCameraShake();
            HandleCursorToggle();
        }
        private void HandleMouseLook()
        {
            // Get mouse input
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;

            // Apply aim sensitivity multiplier when aiming
            if (isAiming)
            {
                mouseX *= aimSensitivityMultiplier;
                mouseY *= aimSensitivityMultiplier;
            }

            // Invert Y axis if needed
            if (invertMouseY)
                mouseY = -mouseY;

            // Apply smoothing if enabled
            if (enableSmoothing)
            {
                Vector2 targetMouseDelta = new Vector2(mouseX, mouseY);
                currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, smoothTime);
                mouseX = currentMouseDelta.x;
                mouseY = currentMouseDelta.y;
            }

            // Horizontal rotation (Y axis) - rotates the player body
            yRotation += mouseX;

            // Vertical rotation (X axis) - rotates the camera
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

            // Apply rotations
            if (playerBody != null)
            {
                playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
            }
            else
            {
                // If no player body, rotate the camera itself for horizontal look
                transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
                return;
            }
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // Apply camera shake to position
            if (enableCameraShake)
            {
                transform.localPosition = cameraShakeOffset;
            }
        }
        private void HandleZoom()
        {
            // Check if aiming
            isAiming = Input.GetKey(aimKey);

            // Check if running (from PlayerController)
            bool isPlayerRunning = playerController != null && playerController.IsRunning();

            // Set target FOV based on aiming state and running state
            if (isAiming)
            {
                targetFOV = zoomedFOV;
            }
            else if (isPlayerRunning)
            {
                targetFOV = runningFOV;
            }
            else
            {
                targetFOV = normalFOV;
            }

            // Smoothly interpolate to target FOV
            currentFOV = Mathf.Lerp(currentFOV, targetFOV, zoomSpeed * Time.deltaTime);
            // Apply FOV to camera
            if (playerCamera != null)
            {
                playerCamera.fieldOfView = currentFOV;
            }
        }

        private void HandleCameraShake()
        {
            if (!enableCameraShake || playerController == null)
            {
                cameraShakeOffset = Vector3.zero;
                return;
            }

            bool isGrounded = playerController.IsGrounded();
            bool isMoving = playerController.IsMoving();
            bool isRunning = playerController.IsRunning();

            // Check for landing shake
            if (isGrounded && !wasGrounded)
            {
                landingShakeTimer = landingShakeDuration;
            }
            wasGrounded = isGrounded;

            // Calculate shake intensity
            float shakeIntensity = 0f;

            // Landing shake (highest priority)
            if (landingShakeTimer > 0f)
            {
                landingShakeTimer -= Time.deltaTime;
                float landingShakeAmount = (landingShakeTimer / landingShakeDuration) * landingShakeIntensity;
                shakeIntensity = Mathf.Max(shakeIntensity, landingShakeAmount);
            }

            // Movement shake
            if (isMoving && isGrounded)
            {
                float movementShake = isRunning ? runShakeIntensity : walkShakeIntensity;
                shakeIntensity = Mathf.Max(shakeIntensity, movementShake);
            }

            // Reduce shake when aiming
            if (isAiming)
            {
                shakeIntensity *= aimSensitivityMultiplier;
            }

            // Generate shake offset
            if (shakeIntensity > 0f)
            {
                shakeTimer += Time.deltaTime * shakeFrequency;

                float shakeX = Mathf.Sin(shakeTimer) * shakeIntensity;
                float shakeY = Mathf.Sin(shakeTimer * 1.3f) * shakeIntensity * 0.7f;
                float shakeZ = Mathf.Sin(shakeTimer * 0.8f) * shakeIntensity * 0.5f;

                cameraShakeOffset = new Vector3(shakeX, shakeY, shakeZ);
            }
            else
            {
                // Smoothly reduce shake to zero
                cameraShakeOffset = Vector3.Lerp(cameraShakeOffset, Vector3.zero, Time.deltaTime * shakeReduction);
            }
        }

        private void HandleCursorToggle()
        {
            // Toggle cursor lock with Escape key
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }

        // Public methods for external control
        public void SetMouseSensitivity(float sensitivityX, float sensitivityY)
        {
            mouseSensitivityX = sensitivityX;
            mouseSensitivityY = sensitivityY;
        }

        public void SetVerticalLimits(float minAngle, float maxAngle)
        {
            minVerticalAngle = minAngle;
            maxVerticalAngle = maxAngle;
        }

        public void ResetRotation()
        {
            xRotation = 0f;
            yRotation = 0f;
            transform.localRotation = Quaternion.identity;
            if (playerBody != null)
                playerBody.rotation = Quaternion.identity;
        }

        public void SetInvertMouse(bool invert)
        {
            invertMouseY = invert;
        }

        public void SetSmoothing(bool enable, float time = 0.1f)
        {
            enableSmoothing = enable;
            smoothTime = time;
        }
    }
}
