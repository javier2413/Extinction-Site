using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public static InputManager instance { get; private set; }

    public InputActionAsset playerControls;

    [SerializeField] private string actionMapName = "Player";

    // Actions dictionary for easy access by name
    public Dictionary<string, InputAction> ActionsDictionary { get; private set; } = new Dictionary<string, InputAction>();

    // Individual actions
    public InputAction LookAction { get; private set; }
    public InputAction MoveAction { get; private set; }
    public InputAction RunningAction { get; private set; }
    public InputAction InteractionAction { get; private set; }
    public InputAction InventoryAction { get; private set; }
    public InputAction PauseAction { get; private set; }
    public InputAction FlashlightAction { get; private set; }
    public InputAction FlashAction { get; private set; }
    public InputAction RechargeAction { get; private set; }
    public InputAction RockThrowAction { get; private set; }
    public InputAction CrouchAction { get; private set; }

    public void SetInteractionTriggered(bool value) => InteractionTriggered = value;

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

    private void Awake()
    {
        // Singleton setup
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);

        InitializeInputActions();
        EnableInputActions();
    }

    private void OnEnable() => EnableInputActions();
    private void OnDisable() => DisableInputActions();

    private void InitializeInputActions()
    {
        var map = playerControls.FindActionMap(actionMapName);

        LookAction = map.FindAction("Look");
        MoveAction = map.FindAction("Move");
        RunningAction = map.FindAction("Running");
        InteractionAction = map.FindAction("Interaction");
        InventoryAction = map.FindAction("Inventory");
        PauseAction = map.FindAction("Pause");
        FlashlightAction = map.FindAction("Flashlight");
        FlashAction = map.FindAction("Flash");
        RechargeAction = map.FindAction("Recharge");
        RockThrowAction = map.FindAction("RockThrow");
        CrouchAction = map.FindAction("Crouch");

        // Populate dictionary
        ActionsDictionary["Look"] = LookAction;
        ActionsDictionary["Move"] = MoveAction;
        ActionsDictionary["Running"] = RunningAction;
        ActionsDictionary["Interaction"] = InteractionAction;
        ActionsDictionary["Inventory"] = InventoryAction;
        ActionsDictionary["Pause"] = PauseAction;
        ActionsDictionary["Flashlight"] = FlashlightAction;
        ActionsDictionary["Flash"] = FlashAction;
        ActionsDictionary["Recharge"] = RechargeAction;
        ActionsDictionary["RockThrow"] = RockThrowAction;
        ActionsDictionary["Crouch"] = CrouchAction;

        // Bind events
        LookAction.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        LookAction.canceled += ctx => LookInput = Vector2.zero;

        MoveAction.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        MoveAction.canceled += ctx => MoveInput = Vector2.zero;

        RunningAction.performed += ctx => RunningValue = ctx.ReadValue<float>();
        RunningAction.canceled += ctx => RunningValue = 0f;

        InteractionAction.started += ctx => InteractionTriggered = true;
        InteractionAction.canceled += ctx => InteractionTriggered = false;

        InventoryAction.started += ctx => InventoryTriggered = true;
        InventoryAction.canceled += ctx => InventoryTriggered = false;

        PauseAction.started += ctx => PauseTriggered = true;
        PauseAction.canceled += ctx => PauseTriggered = false;

        FlashlightAction.started += ctx => FlashlightTriggered = true;
        FlashlightAction.canceled += ctx => FlashlightTriggered = false;

        FlashAction.started += ctx => FlashTriggered = true;
        FlashAction.canceled += ctx => FlashTriggered = false;

        RechargeAction.started += ctx => RechargeTriggered = true;
        RechargeAction.canceled += ctx => RechargeTriggered = false;

        CrouchAction.started += ctx => CrouchTriggered = true;
        CrouchAction.canceled += ctx => CrouchTriggered = false;
    }
    public InputAction GetActionByName(string name)
    {
        if (ActionsDictionary.TryGetValue(name, out var action))
            return action;

        Debug.LogWarning($"InputManager: Action '{name}' not found.");
        return null;
    }

    private void EnableInputActions()
    {
        foreach (var action in ActionsDictionary.Values)
            action.Enable();
    }

    private void DisableInputActions()
    {
        foreach (var action in ActionsDictionary.Values)
            action.Disable();
    }

    // Setters
    public void SetFlashlightTriggered(bool value) => FlashlightTriggered = value;
    public void SetFlashTriggered(bool value) => FlashTriggered = value;
    public void SetRechargeTriggered(bool value) => RechargeTriggered = value;
    public void SetCrouchTriggered(bool value) => CrouchTriggered = value;
}




