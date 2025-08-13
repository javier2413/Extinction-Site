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
    public LayerMask doorLayer;

    [Header("Flashlight Settings")]
    public FlashlightSystem flashlightSystem;
    public string flashlightSound;
    public string getFlashlightSound;

    // Private fields
    private float verticalVelocity;
    private float cameraPitch;
    private bool isCrouching = false;
    private bool isFlashlight = false;

    // Input placeholders
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintInput;
    private bool crouchToggleInput;
    private bool interactInput;
    private bool flashlightToggleInput;

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();
        if (flashlightSystem == null)
            flashlightSystem = GetComponentInChildren<FlashlightSystem>();

        Vector3 camPos = playerCamera.localPosition;
        camPos.y = standingHeight - 0.1f;
        playerCamera.localPosition = camPos;
    }

    void Update()
    {
        GatherInput();

        HandleLook();
        HandleMovement();
        HandleCrouch();
        HandleFlashlightToggle();
        TryInteract();

    }

    //gather inputs to move camera
    void GatherInput()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        sprintInput = Input.GetKey(KeyCode.LeftShift);
        crouchToggleInput = Input.GetKeyDown(KeyCode.C);
        interactInput = Input.GetKeyDown(KeyCode.E);
        flashlightToggleInput = Input.GetKeyDown(KeyCode.F);
    }

    //Look around with camera
    void HandleLook()
    {
        cameraPitch -= lookInput.y * lookSpeed;
        cameraPitch = Mathf.Clamp(cameraPitch, bottomClamp, topClamp);
        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);

        transform.Rotate(Vector3.up * lookInput.x * lookSpeed);
    }

    //player movement handle
    void HandleMovement()
    {
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        float speed = isCrouching ? crouchSpeed : (sprintInput ? runSpeed : walkSpeed);
        Vector3 velocity = moveDirection.normalized * speed;

        playerAnimations?.SetMovementParameters(moveInput.x, moveInput.y);

        bool isRunning = !isCrouching && sprintInput && (moveInput.magnitude > 0.1f);
        playerAnimations?.RunningAnimation(isRunning);

        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        velocity.y = verticalVelocity;

        Debug.Log($"IsGrounded: {controller.isGrounded}, VerticalVelocity: {verticalVelocity}");

        controller.Move(velocity * Time.deltaTime);
    }


    void HandleCrouch()
    {
        if (crouchToggleInput)
            isCrouching = !isCrouching;

        controller.height = isCrouching ? crouchHeight : standingHeight;
        controller.center = new Vector3(0, controller.height / 2f, 0);

        Vector3 camPos = playerCamera.localPosition;
        camPos.y = isCrouching ? crouchHeight - 0.1f : standingHeight - 0.1f;
        playerCamera.localPosition = camPos;
    }

    public void PlayFootstep()
    {
        AudioManager.instance.Play("FootstepSound");
    }

    //Flashlight

    void HandleFlashlightToggle()
    {
        if (flashlightToggleInput && flashlightSystem != null)
        {
            FlashlightSwitch();
            flashlightToggleInput = false; // reset input flag after toggling
        }
    }

    private void FlashlightSwitch()
    {
        isFlashlight = !isFlashlight;
        playerAnimations?.SetFlashlightIdle(isFlashlight);
        AudioManager.instance.Play(getFlashlightSound);
        flashlightSystem.ToggleFlashlight(isFlashlight);
    }

    public void FlashlightDropAnimationEvent()
    {
        AudioManager.instance.Play(getFlashlightSound);
        //InventoryManager.instance.SetFlashlightActiveOnPlayer(false);
    }

    public void FlashlightSwitchAniamtionEvent()
    {
        // Code that should run when this event fires, e.g. play a sound or toggle some animation state
        AudioManager.instance.Play(getFlashlightSound);
        flashlightSystem.ToggleFlashlight(false);
    }

    public void FlashlightSystemAnimationEvent()
    {
        // Code for this event, e.g. toggle the flashlight's light on/off
        AudioManager.instance.Play(flashlightSound);
        flashlightSystem.ToggleFlashlight(isFlashlight);
    }

    //Door

    void TryInteract()
    {
        if (!interactInput) return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            var interactive = hit.collider.GetComponent<InteractiveObject>();
            if (interactive != null)
            {
                interactive.Interact(gameObject);
            }
        }
    }
}

