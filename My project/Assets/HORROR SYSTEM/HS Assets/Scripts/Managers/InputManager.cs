using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance { get; private set; }

    [Header("Input Action Asset")]
    public InputActionAsset playerControls;

    [Header("Action Map Name")]
    [SerializeField] private string actionMapName = "Player";

    // Action names
    [SerializeField] private string look = "Look";
    [SerializeField] private string move = "Move";
    [SerializeField] private string running = "Running";
    [SerializeField] private string interaction = "Interaction";
    [SerializeField] private string inventory = "Inventory";
    [SerializeField] private string pause = "Pause";
    [SerializeField] private string flashlight = "Flashlight";
    [SerializeField] private string flash = "Flash";
    [SerializeField] private string recharge = "Recharge";
    [SerializeField] private string crouch = "Crouch";
    [SerializeField] private string rockThrow = "RockThrow";

    // Actions
    private InputAction lookAction;
    private InputAction moveAction;
    private InputAction runningAction;
    private InputAction interactionAction;
    private InputAction inventoryAction;
    private InputAction pauseAction;
    private InputAction flashlightAction;
    private InputAction flashAction;
    private InputAction rechargeAction;
    private InputAction crouchAction;
    private InputAction rockThrowAction;

    // Input values
    public Vector2 LookInput { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public float RunningValue { get; private set; }
    public bool InteractionTriggered { get; private set; }
    public bool InventoryTriggered { get; private set; }
    public bool PauseTriggered { get; private set; }
    public bool FlashlightTriggered { get; private set; }
    public bool FlashTriggered { get; private set; }
    public bool RechargeTriggered { get; private set; }
    public bool CrouchTriggered { get; private set; }
    public bool RockThrowTriggered { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        InitializeInputActions();
    }

    private void OnEnable() => EnableInputActions();
    private void OnDisable() => DisableInputActions();

    private void InitializeInputActions()
    {
        var map = playerControls.FindActionMap(actionMapName);

        lookAction = map.FindAction(look);
        moveAction = map.FindAction(move);
        runningAction = map.FindAction(running);
        interactionAction = map.FindAction(interaction);
        inventoryAction = map.FindAction(inventory);
        pauseAction = map.FindAction(pause);
        flashlightAction = map.FindAction(flashlight);
        flashAction = map.FindAction(flash);
        rechargeAction = map.FindAction(recharge);
        crouchAction = map.FindAction(crouch);
        rockThrowAction = map.FindAction(rockThrow);

        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        lookAction.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        lookAction.canceled += ctx => LookInput = Vector2.zero;

        moveAction.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => MoveInput = Vector2.zero;

        runningAction.performed += ctx => RunningValue = ctx.ReadValue<float>();
        runningAction.canceled += ctx => RunningValue = 0f;

        interactionAction.started += ctx => InteractionTriggered = true;
        interactionAction.canceled += ctx => InteractionTriggered = false;

        inventoryAction.started += ctx => InventoryTriggered = true;
        inventoryAction.canceled += ctx => InventoryTriggered = false;

        pauseAction.started += ctx => PauseTriggered = true;
        pauseAction.canceled += ctx => PauseTriggered = false;

        flashlightAction.started += ctx => FlashlightTriggered = true;
        flashlightAction.canceled += ctx => FlashlightTriggered = false;

        flashAction.started += ctx => FlashTriggered = true;
        flashAction.canceled += ctx => FlashTriggered = false;

        rechargeAction.started += ctx => RechargeTriggered = true;
        rechargeAction.canceled += ctx => RechargeTriggered = false;

        crouchAction.started += ctx => CrouchTriggered = true;
        crouchAction.canceled += ctx => CrouchTriggered = false;

        rockThrowAction.started += ctx => RockThrowTriggered = true;
        rockThrowAction.canceled += ctx => RockThrowTriggered = false;
    }

    private void EnableInputActions()
    {
        lookAction.Enable();
        moveAction.Enable();
        runningAction.Enable();
        interactionAction.Enable();
        inventoryAction.Enable();
        pauseAction.Enable();
        flashlightAction.Enable();
        flashAction.Enable();
        rechargeAction.Enable();
        crouchAction.Enable();
        rockThrowAction.Enable();
    }

    private void DisableInputActions()
    {
        lookAction.Disable();
        moveAction.Disable();
        runningAction.Disable();
        interactionAction.Disable();
        inventoryAction.Disable();
        pauseAction.Disable();
        flashlightAction.Disable();
        flashAction.Disable();
        rechargeAction.Disable();
        crouchAction.Disable();
        rockThrowAction.Disable();
    }

    // Optional setters
    public void SetInteractionTriggered(bool value) => InteractionTriggered = value;
    public void SetInventoryTriggered(bool value) => InventoryTriggered = value;
    public void SetPauseTriggered(bool value) => PauseTriggered = value;
    public void SetFlashlightTriggered(bool value) => FlashlightTriggered = value;
    public void SetFlashTriggered(bool value) => FlashTriggered = value;
    public void SetRechargeTriggered(bool value) => RechargeTriggered = value;
    public void SetCrouchTriggered(bool value) => CrouchTriggered = value;
    public void SetRockThrowTriggered(bool value) => RockThrowTriggered = value;
}





