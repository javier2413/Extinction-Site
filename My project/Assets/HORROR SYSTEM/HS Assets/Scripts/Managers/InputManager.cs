using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance { get; private set; }

    [Header("Input Action Asset")]
    public InputActionAsset playerControls;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string look = "Look";
    [SerializeField] private string move = "Move";
    [SerializeField] private string running = "Running";
    [SerializeField] private string interaction = "Interaction";
    [SerializeField] private string inventory = "Inventory";
    [SerializeField] private string pause = "Pause";
    [SerializeField] private string flashlight = "Flashlight";
    [SerializeField] private string aiming = "Aiming";
    [SerializeField] private string backmenu = "Backmenu";
    [SerializeField] private string combat = "Combat";
   

    private InputAction lookAction;
    private InputAction moveAction;
    private InputAction runningAction;
    private InputAction interactionAction;
    private InputAction inventoryAction;
    private InputAction pauseAction;
    private InputAction flashlightAction;
    private InputAction aimingAction;
    private InputAction backmenuAction;
    private InputAction combatAction;


    public Vector2 LookInput { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public float RunningValue { get; private set; }
    public float AimingValue { get; private set; }
    public float CombatValue { get; private set; }
    public bool InteractionTriggered { get; private set; }
    public bool BackMenuTriggered { get; private set; }
    public bool InventoryTriggered { get; private set; }
    public bool PauseTriggered { get; private set; }
    public bool FlashlightTriggered { get; private set; }
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeInputActions();
    }

    private void OnEnable()
    {
        EnableInputActions();
    }

    private void OnDisable()
    {
        DisableInputActions();
    }

    private void InitializeInputActions()
    {
        lookAction = playerControls.FindActionMap(actionMapName).FindAction(look);
        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        runningAction = playerControls.FindActionMap(actionMapName).FindAction(running);
        interactionAction = playerControls.FindActionMap(actionMapName).FindAction(interaction);
        inventoryAction = playerControls.FindActionMap(actionMapName).FindAction(inventory);
        pauseAction = playerControls.FindActionMap(actionMapName).FindAction(pause);
        flashlightAction = playerControls.FindActionMap(actionMapName).FindAction(flashlight);
        aimingAction = playerControls.FindActionMap(actionMapName).FindAction(aiming);
        backmenuAction = playerControls.FindActionMap(actionMapName).FindAction(backmenu);
        combatAction = playerControls.FindActionMap(actionMapName).FindAction(combat);
       

        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => LookInput = Vector2.zero;

        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        runningAction.performed += context => RunningValue = context.ReadValue<float>();
        runningAction.canceled += context => RunningValue = 0f;

        aimingAction.performed += context => AimingValue = context.ReadValue<float>();
        aimingAction.canceled += context => AimingValue = 0f;

        combatAction.performed += context => CombatValue = context.ReadValue<float>();
        combatAction.canceled += context => CombatValue = 0f;

        interactionAction.started += context => InteractionTriggered = true;
        interactionAction.canceled += context => InteractionTriggered = false;

        inventoryAction.started += context => InventoryTriggered = true;
        inventoryAction.canceled += context => InventoryTriggered = false;

        pauseAction.started += context => PauseTriggered = true;
        pauseAction.canceled += context => PauseTriggered = false;

        backmenuAction.started += context => BackMenuTriggered = true;
        backmenuAction.canceled += context => BackMenuTriggered = false;

        flashlightAction.started += context => FlashlightTriggered = true;
        flashlightAction.canceled += context => FlashlightTriggered = false;

        
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
        backmenuAction.Enable();
        aimingAction.Enable();
        combatAction.Enable();
       
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
        aimingAction.Disable();
        combatAction.Disable();
        backmenuAction.Disable();
      
    }

    public void SetInteractionTriggered(bool value)
    {
        InteractionTriggered = value;
    }

    public void SetBackMenuTriggered(bool value)
    {
        BackMenuTriggered = value;
    }

    public void SetInventoryTriggered(bool value)
    {
        InventoryTriggered = value;
    }

    public void SetPauseTriggered(bool value)
    {
        PauseTriggered = value;
    }

    public void SetFlashlightTriggered(bool value)
    {
        FlashlightTriggered = value;
    }

    public void ResetCombatValue()
    {
        CombatValue = 0f;
    }
}
