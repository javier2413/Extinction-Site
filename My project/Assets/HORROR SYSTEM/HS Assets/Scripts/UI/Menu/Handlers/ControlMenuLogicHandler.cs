using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using System.Linq;



    public class ControlMenuLogicHandler : MonoBehaviour
    {
        [Header("Input Action Asset")]
        public InputActionAsset playerControls;

        [Header("Rebinding Settings")]
        public Button[] rebindButtons;

    public string[] actionsToRebind = {
        "Pause/<Keyboard>/escape",
        "Running/<Keyboard>/rightShift",
        "Move/up/<Keyboard>/W",
        "Move/down/<Keyboard>/S",
        "Move/left/<Keyboard>/A",
        "Move/right/<Keyboard>/D",
        "Inventory/<Keyboard>/I",
        "Interact/<Keyboard>/E",
        "Flashlight/<Keyboard>/F",
        "Flash/<Keyboard>/space",
        "Recharge/<Keyboard>/R",
        "RockThrown/<Keyboard>/H"
    };

    [Header("Mouse Sensitivity Settings")]
        public Slider sensitivitySlider;
        public TMP_Text sensitivityValueText;



        private void Start()
    {
        // Pause default override
        var pauseAction = playerControls.FindAction("Pause");
        if (pauseAction != null)
        {
            string savedPauseBinding = PlayerPrefs.GetString(pauseAction.id.ToString(), string.Empty);
            if (string.IsNullOrEmpty(savedPauseBinding))
            {
                int pauseBindingIndex = FindBindingIndexByPath(pauseAction, "<Keyboard>/escape");
                if (pauseBindingIndex == -1)
                    pauseBindingIndex = 0; // fallback if not found

                pauseAction.ApplyBindingOverride(pauseBindingIndex, "<Keyboard>/escape");
                Debug.Log($"Pause binding override applied at index {pauseBindingIndex} with <Keyboard>/escape");
            }
        }

        // Running default override
        var runningAction = playerControls.FindAction("Running");
        if (runningAction != null)
        {
            string savedRunningBinding = PlayerPrefs.GetString(runningAction.id.ToString(), string.Empty);
            if (string.IsNullOrEmpty(savedRunningBinding))
            {
                int runningBindingIndex = FindBindingIndexByPath(runningAction, "<Keyboard>/rightShift");
                if (runningBindingIndex == -1)
                    runningBindingIndex = 0; // fallback if not found

                runningAction.ApplyBindingOverride(runningBindingIndex, "<Keyboard>/rightShift");
                Debug.Log($"Running binding override applied at index {runningBindingIndex} with <Keyboard>/rightShift");
            }
        }

        // Load any saved bindings (overrides)
        LoadRebindingSettings();

        InitializeRebindButtonsLogic();
        InitializeMouseSensitivitySliderLogic();
    }

    private int FindBindingIndexByPath(InputAction action, string expectedPathStart)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            string bindingPath = action.bindings[i].effectivePath;
            if (!string.IsNullOrEmpty(bindingPath) && bindingPath.StartsWith(expectedPathStart, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }
        return -1;
    }



    private void InitializeRebindButtonsLogic()
    {
        for (int i = 0; i < rebindButtons.Length; i++)
        {
            int index = i;

            if (rebindButtons[i] == null)
            {
                Debug.LogWarning($"Rebind button at index {i} is null and will be skipped.");
                continue;
            }

            UpdateButtonText(index);
            rebindButtons[i].onClick.AddListener(() => StartRebinding(index));
        }
    }

    public void LoadRebindingSettings()
    {
        foreach (var action in playerControls)
        {
            string bindingJson = PlayerPrefs.GetString(action.id.ToString(), string.Empty);
            if (!string.IsNullOrEmpty(bindingJson))
            {
                action.LoadBindingOverridesFromJson(bindingJson);
            }
        }
    }

    private void UpdateButtonText(int buttonIndex)
    {
        if (buttonIndex >= actionsToRebind.Length || buttonIndex >= rebindButtons.Length)
        {
            Debug.LogWarning($"Invalid index {buttonIndex} in UpdateButtonText.");
            return;
        }

        if (rebindButtons[buttonIndex] == null)
        {
            Debug.LogWarning($"Rebind button at index {buttonIndex} is null.");
            return;
        }

        string[] parts = actionsToRebind[buttonIndex].Split('/');
        string actionName = parts[0];
        string bindingPath = string.Join("/", parts.Skip(1)); // in case binding path has slashes

        var action = playerControls.FindAction(actionName);
        if (action != null)
        {
            int bindingIndex = FindBindingIndexByPath(action, bindingPath);
            if (bindingIndex != -1)
            {
                var binding = action.bindings[bindingIndex];
                var buttonText = rebindButtons[buttonIndex].GetComponentInChildren<TMPro.TMP_Text>();
                if (buttonText != null)
                {
                    string humanReadable = InputControlPath.ToHumanReadableString(
                        binding.effectivePath,
                        InputControlPath.HumanReadableStringOptions.OmitDevice);

                    buttonText.text = humanReadable;
                    Debug.Log($"Button {buttonIndex} text set to: {humanReadable}");
                }
            }
        }
    }



    private int GetBindingIndex(InputAction action, string bindingName)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (bindingName == null ||
                (!action.bindings[i].isPartOfComposite && action.bindings[i].name.Equals(bindingName, StringComparison.OrdinalIgnoreCase)) ||
                (action.bindings[i].isPartOfComposite && action.bindings[i].name.Equals(bindingName, StringComparison.OrdinalIgnoreCase)))
            {
                return i;
            }
        }
        return -1;
    }


    public void StartRebinding(int actionIndex)
{
    if (actionIndex >= actionsToRebind.Length || actionIndex >= rebindButtons.Length)
        return;

    string[] parts = actionsToRebind[actionIndex].Split('/');
    string actionName = parts[0];
    string bindingPath = string.Join("/", parts.Skip(1)); // handle slashes

    var action = playerControls.FindAction(actionName);
    if (action == null) return;

    int bindingIndex = FindBindingIndexByPath(action, bindingPath);
    if (bindingIndex == -1) return;

    var buttonText = rebindButtons[actionIndex].GetComponentInChildren<TMPro.TMP_Text>();
    if (buttonText != null)
        buttonText.text = "Waiting for input";

    action.Disable();

    var rebinding = action.PerformInteractiveRebinding(bindingIndex);

    if (actionName == "Pause")
    {
        rebinding = rebinding
            .WithControlsExcluding("<Mouse>/leftButton")
            .WithControlsExcluding("<Mouse>/rightButton")
            .WithControlsExcluding("<Mouse>/middleButton");
    }

    rebinding
        .OnComplete(operation =>
        {
            action.Enable();
            PlayerPrefs.SetString(action.id.ToString(), action.SaveBindingOverridesAsJson());
            operation.Dispose();
            UpdateButtonText(actionIndex);
            EventSystem.current.SetSelectedGameObject(rebindButtons[actionIndex].gameObject);
        })
        .Start();
}



    private void InitializeMouseSensitivitySliderLogic()
    {
        float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 5f);
        sensitivitySlider.value = savedSensitivity;
        sensitivityValueText.text = $"{(int)sensitivitySlider.value}";

        sensitivitySlider.onValueChanged.AddListener(
            sensitivity => {
                sensitivityValueText.text = $"{(int)sensitivity}";
                PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
                PlayerPrefs.Save();
            }
        );
    }
}