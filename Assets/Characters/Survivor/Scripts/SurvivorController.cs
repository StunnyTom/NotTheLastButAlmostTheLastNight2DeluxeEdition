using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif


namespace SurvivorSystem
{
    /// <summary>
    /// Controller for Survivor players
    /// Based on MonsterController but without attack
    /// </summary>
    public class SurvivorController : NetworkBehaviour, IDamageable
    {
        // ... (Headers omitted for brevity, they remain unchanged) ...

        [Header("Movement Settings")]
        [Range(0f, 20f)] public float walkSpeed = 6f; // Doubled
        [Range(0f, 30f)] public float sprintSpeed = 10f; // Doubled
        [Range(0f, 20f)] public float crouchSpeed = 3f; // Doubled
        [Range(0f, 15f)] public float jumpSpeed = 6f; // Doubled
        [Range(0f, 50f)] public float gravity = 20f; // Increased for snappier fall

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

        [Header("Inventory")]
        public Inventory playerInventory;
        public Transform objectHandler;

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
        private OutlineTarget currentOutlined;
        private bool isDead = false;

        // Inventory helper
        private bool objectInHand = false;
        private UsableItem equippedItem = null;

        // Ragdoll
        private Rigidbody[] ragdollRigidbodies;
        private Collider[] ragdollColliders;


        [Header("Debug Options")]
        // Tests
        public bool isStatic = false;

        // Safely get an inventory reference (serialized field preferred, fallback to singleton)
        private Inventory GetInventory()
        {
            if (playerInventory != null) return playerInventory;
            return Inventory.Instance;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                // DISABLE REMOTE PLAYER
                this.enabled = false;
                if (playerCamera) playerCamera.gameObject.SetActive(false);
                if (thirdPersonCamera) thirdPersonCamera.gameObject.SetActive(false);
                var cams = GetComponentsInChildren<Camera>();
                foreach (var c in cams) c.enabled = false;
                var listener = GetComponentInChildren<AudioListener>();
                if (listener) listener.enabled = false;
                
                Debug.Log($"[SurvivorController] Remote Player {OwnerClientId}: Disabled controls and cameras.");
            }
            else
            {
                // LOCAL PLAYER SETUP
                SceneManager.sceneLoaded += OnSceneLoaded;
                ConfigureControlsForScene();
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ConfigureControlsForScene();
        }

        private void ConfigureControlsForScene()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            bool isLobby = sceneName.Contains("Lobby") || sceneName.Contains("Menu");

            if (isLobby)
            {
                // LOBBY MODE: Cursor Free, No Movement, NO CAMERA (Prevent UI occlusion)
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                isLookEnabled = false;
                isMoveEnabled = false;
                
                // Disable character cameras so they don't block the Main Menu
                if (playerCamera) playerCamera.gameObject.SetActive(false);
                if (thirdPersonCamera) thirdPersonCamera.gameObject.SetActive(false);

                Debug.Log($"[SurvivorController] Local Player {OwnerClientId}: LOBBY MODE (Controls & Cameras Disabled)");
            }
            else
            {
                // GAME MODE: Cursor Locked, Movement Active, Camera Active
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                isLookEnabled = true;
                isMoveEnabled = true;
                
                // Initialize Camera correctly based on isFirstPerson flag
                ApplyCameraState();

                Debug.Log($"[SurvivorController] Local Player {OwnerClientId}: GAME MODE (Controls Enabled, Cursor Locked)");
            }
        }

        void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            // === RAGDOLL SETUP ===
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            ragdollColliders = GetComponentsInChildren<Collider>();
            SetRagdoll(false);

            if (isStatic) return;

            if (playerCamera != null)
                cam = playerCamera.GetComponent<Camera>();

            // REMOVED: Cursor locking moved to ConfigureControlsForScene
            // Cursor.lockState = CursorLockMode.Locked; 
            // Cursor.visible = false;

            rotX = transform.rotation.eulerAngles.y;
            rotY = playerCamera != null ? playerCamera.localRotation.eulerAngles.x : 0;
            xVelocity = rotX;
            yVelocity = rotY;
        }


        void Update()
            {
                if (isStatic || isDead) return;
                CheckGrounded();
                HandleLook();
                HandleMovement();
                UpdateAnimations();
                HandleCameraToggle();

                HandleInteraction();
                HandleOutlineRaycast();
                HandleChangeSelectedItem();
            }

        void HandleInteraction()
        {
            // Pick item
            bool pickPressed = false;
        #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            if (Keyboard.current != null)
            pickPressed = Keyboard.current.eKey.wasPressedThisFrame;
        #else
            pickPressed = Input.GetKeyDown(KeyCode.E);
        #endif

            if (pickPressed)
            TryPickItem();

            // Use item
            bool usePressed = false;
        #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            if (Mouse.current != null)
            usePressed = Mouse.current.leftButton.wasPressedThisFrame;
        #else
            usePressed = Input.GetMouseButtonDown(0);
        #endif

            if (usePressed)
            HandleItemUse();

            // Drop item
            bool dropPressed = false;
        #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            if (Keyboard.current != null)
            dropPressed = Keyboard.current.qKey.wasPressedThisFrame;
        #else
            dropPressed = Input.GetKeyDown(KeyCode.Q);
        #endif

            if (dropPressed)
            DropSelectedItem();
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
                ApplyCameraState();
            }

            if (!isFirstPerson && thirdPersonCamera != null)
            {
                Vector3 targetPosition = transform.position - transform.forward * thirdPersonDistance + Vector3.up * thirdPersonHeight;
                thirdPersonCamera.position = Vector3.Lerp(thirdPersonCamera.position, targetPosition, Time.deltaTime * 5f);
                thirdPersonCamera.LookAt(transform.position + Vector3.up * 1.5f);
            }
        }

        private void ApplyCameraState()
        {
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



        public void SetControl(bool newState)
        {
            isLookEnabled = newState;
            isMoveEnabled = newState;
        }

        public bool IsSprinting => isSprinting;
        public bool IsCrouching => isCrouching;
        public Vector2 GetMoveInput => moveInput;
    

        private void TryPickItem()
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, 2f))
            {
                if (!hit.collider.CompareTag("Usable")) return;

                UsableItem item = hit.collider.GetComponent<UsableItem>();
                if (item != null)
                {
                    Inventory inv = GetInventory();
                    if (inv == null) return;
                    if (item is PressurePlateItem plate && plate.IsPressed()) return;

                    int slotIndex = inv.AddItem(item);

                    if (slotIndex != -1)
                    {
                        Debug.Log("Item picked: " + item.itemName);

                        if (!objectInHand)
                        {
                            // Seulement équiper si la main est vide !
                            EquipSelectedItem();
                        }
                        else
                        {
                            // Main occupée => on place juste dans l'inventaire
                            // MAIS surtout : on bloque tout changement de sélection
                            inv.ForceKeepCurrentSelection();

                            item.gameObject.SetActive(false);
                            item.transform.SetParent(null);

                            EquipSelectedItem();
                        }
                    }
                    else
                    {
                        Debug.Log("Inventaire plein.");
                    }
                }
            }
        }



        private void HandleItemUse()
        {
            bool usePressed = false;

        #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            if (Mouse.current != null)
                usePressed = Mouse.current.leftButton.wasPressedThisFrame;
        #else
            usePressed = Input.GetMouseButtonDown(0);
        #endif

            if (!usePressed)
                return;

            Inventory inv = GetInventory();
            if (inv == null)
            {
                Debug.LogWarning("Inventory reference is null. Assign 'playerInventory' in the inspector or ensure Inventory.Instance is initialized.");
                return;
            }

            UsableItem item = inv.GetSelectedItem();
            if (item == null) return;

            // Sécurité anti-désync
            if (item != equippedItem)
            {
                Debug.LogWarning("Inventory desync detected → resyncing");
                EquipSelectedItem();
                return;
            }

            item.Use();
        }

        private void HandleChangeSelectedItem()
        {
            #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            if (Mouse.current == null) return;
            float scrollDelta = Mouse.current.scroll.y.ReadValue();
            #else
            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            #endif

            if (scrollDelta != 0f)
            {
                Inventory inv = GetInventory();
                if (inv == null)
                {
                    Debug.LogWarning("Inventory reference is null. Assign 'playerInventory' in the inspector or ensure Inventory.Instance is initialized.");
                    return;
                }

                if (scrollDelta > 0f)
                    inv.NextItem();
                else
                    inv.PreviousItem();

                // Mettre à jour l'affichage de l'objet en main
                EquipSelectedItem();
            }
        }


        private void DropSelectedItem()
        {
            Inventory inv = GetInventory();
            if (inv == null)
            {
                Debug.LogWarning("Inventory reference is null.");
                return;
            }

            UsableItem itemToDrop = inv.GetSelectedItem();
            if (itemToDrop == null) return;
            if (itemToDrop is GunItem) return; // On ne peut pas drop les armes à feu 

            // On enlève de l'inventaire et on récupère l'objet supprimé
            UsableItem removed = inv.RemoveSelectedItem();

            // Si c'est bien l'objet que l'on vient de supprimer (sécurité)
            if (removed != null)
            {
                // On remet l'objet physiquement dans la scène devant le joueur
                removed.transform.SetParent(null);
                removed.transform.position = transform.position + transform.forward * 1f;
                removed.gameObject.SetActive(true);

                // Si c'était une plaque de pression, restore son état physique
                if (removed is PressurePlateItem pressurePlate)
                {
                    pressurePlate.transform.rotation = Quaternion.identity;
                    pressurePlate.OnDroppedOrUsedByPlayer();
                    pressurePlate.RegisterRestingState(); // <-- IMPORTANT
                }

                // Nettoyage visuel / état main
                if (equippedItem == removed)
                {
                    equippedItem = null;
                    objectInHand = false;
                }
            }
            else
            {
                Debug.LogWarning("RemoveSelectedItem returned null while trying to drop.");
            }
        }


        // Équipe visuellement l'objet actuellement sélectionné dans l'inventaire
        private void EquipSelectedItem()
        {
            Inventory inv = GetInventory();
            if (inv == null) return;

            UsableItem selected = inv.GetSelectedItem();

            // Si la sélection n'a pas changé (déjà équipé) : rien à faire
            if (selected == equippedItem) return;

            // Désactiver l'ancien équipé proprement (sans le supprimer de l'inventaire)
            if (equippedItem != null)
            {
                equippedItem.gameObject.SetActive(false);
                equippedItem.transform.SetParent(null);
            }

            equippedItem = selected;

            if (equippedItem == null)
            {
                objectInHand = false;
                return;
            }

            if (objectHandler != null)
            {
                // Parent proprement l'objet de l'inventaire à la main
                equippedItem.transform.SetParent(objectHandler, false);
                equippedItem.transform.localPosition = Vector3.zero;
                equippedItem.transform.localRotation = Quaternion.identity;

                if (equippedItem is GunItem)
                    equippedItem.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

                if (equippedItem is PressurePlateItem)
                {
                    equippedItem.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                    equippedItem.transform.localPosition = new Vector3(0f, -0.1f, 0.2f);
                }

                equippedItem.gameObject.SetActive(true);
                objectInHand = true;
            }
            else
            {
                // Si pas de handler, on cache l'objet (consistant)
                equippedItem.gameObject.SetActive(false);
                objectInHand = false;
            }
        }


        void HandleOutlineRaycast()
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, 3f))
            {
                OutlineTarget target = hit.collider.GetComponent<OutlineTarget>();
                if (target == null)
                    target = hit.collider.GetComponentInParent<OutlineTarget>();

                if (target != null)
                {
                    if (currentOutlined != null && currentOutlined != target)
                        currentOutlined.SetOutlined(false);

                    currentOutlined = target;
                    currentOutlined.SetOutlined(true);
                    return;
                }
            }

            if (currentOutlined != null)
            {
                currentOutlined.SetOutlined(false);
                currentOutlined = null;
            }
        }

        private void SetRagdoll(bool enabled)
        {
            // Animator
            if (animator != null)
                animator.enabled = !enabled;

            // CharacterController
            if (characterController != null)
                characterController.enabled = !enabled;

            foreach (var rb in ragdollRigidbodies)
            {
                // On ignore le rigidbody principal (souvent sur le root)
                if (rb.gameObject == gameObject) continue;

                rb.isKinematic = !enabled;
            }

            foreach (var col in ragdollColliders)
            {
                // On garde le collider principal du joueur désactivé à la mort
                if (col.gameObject == gameObject) continue;

                col.enabled = enabled;
            }
        }


        public void OnShot()
        {
            if (isDead) return;
            isDead = true;

            Debug.Log("Survivor shot → dead");

            // Coupe toute logique de gameplay
            SetControl(false);
            moveDirection = Vector3.zero;

            // ACTIVE LE RAGDOLL
            SetRagdoll(true);
        }


    }

}