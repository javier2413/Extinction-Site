using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public CharacterController controller;
    public Transform playerCamera;

    [Header("Animation")]
    public PlayerAnimations playerAnimations;

    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float crouchSpeed = 1.5f;
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float gravity = -15f;

    [Header("Camera Settings")]
    public float lookSpeed = 1f;
    public float topClamp = 90f;
    public float bottomClamp = -90f;

    [Header("Interaction")]
    public float interactionDistance = 3f;
    public LayerMask interactionLayer;
    public GameObject pickUpCanvas;

    [Header("Flashlight Settings")]
    public FlashlightSystem flashlightSystem;

    private float verticalVelocity;
    private float cameraPitch;
    private bool isCrouching = false;
    private bool isFlashlightOn = false;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintInput;
    private bool crouchToggleInput;
    private bool interactInput;
    private bool flashlightToggleInput;

    void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();

        Vector3 camPos = playerCamera.localPosition;
        camPos.y = standingHeight - 0.1f;
        playerCamera.localPosition = camPos;

        // Make sure controller center is correct
        controller.center = new Vector3(0, standingHeight / 2, 0);
    }

    void Update()
    {
        GatherInput();
        HandleLook();
        HandleMovement();
        HandleCrouch();
        HandleFlashlightToggle();
        HandleInteract();
    }

    void GatherInput()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        sprintInput = Input.GetKey(KeyCode.LeftShift);
        crouchToggleInput = Input.GetKeyDown(KeyCode.C);
        interactInput = Input.GetKeyDown(KeyCode.E);
        flashlightToggleInput = Input.GetKeyDown(KeyCode.F);
    }

    void HandleLook()
    {
        cameraPitch -= lookInput.y * lookSpeed;
        cameraPitch = Mathf.Clamp(cameraPitch, bottomClamp, topClamp);
        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * lookInput.x * lookSpeed);
    }

    void HandleMovement()
    {
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        float speed = isCrouching ? crouchSpeed : (sprintInput ? runSpeed : walkSpeed);
        Vector3 velocity = moveDirection.normalized * speed;

        // Animation
        playerAnimations?.SetMovementParameters(moveInput.x, moveInput.y);
        bool isRunning = !isCrouching && sprintInput && moveInput.magnitude > 0.1f;
        playerAnimations?.RunningAnimation(isRunning);

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f; // keep grounded

        verticalVelocity += gravity * Time.deltaTime;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    void HandleCrouch()
    {
        if (crouchToggleInput) isCrouching = !isCrouching;

        controller.height = isCrouching ? crouchHeight : standingHeight;
        controller.center = new Vector3(0, controller.height / 2, 0);

        Vector3 camPos = playerCamera.localPosition;
        camPos.y = isCrouching ? crouchHeight - 0.1f : standingHeight - 0.1f;
        playerCamera.localPosition = camPos;
    }

    void HandleFlashlightToggle()
    {
        if (flashlightToggleInput && flashlightSystem != null)
        {
            isFlashlightOn = !flashlightSystem.IsOn();
            flashlightSystem.ToggleFlashlight(isFlashlightOn);
            playerAnimations?.SetFlashlightIdle(isFlashlightOn);
        }
    }

    void HandleInteract()
    {
        if (!interactInput) return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactionLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
                interactable.Interact(gameObject);
        }
    }
}




