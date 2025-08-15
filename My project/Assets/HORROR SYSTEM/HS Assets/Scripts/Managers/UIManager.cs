using UnityEngine;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI Panels")]
    public GameObject interactionPanel;
    public GameObject inventoryPanel;
    public GameObject pauseMenuPanel;

    [Header("System")]
    public MenuController menuSystem;

    private bool isInventoryPanelActive;
    private bool isPausePanelActive;
    private bool wasInventoryPanelActiveBeforePause;

    private void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        interactionPanel.SetActive(true);
        inventoryPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);

        isInventoryPanelActive = false;
        isPausePanelActive = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void PlayerDisable()
    {
        if (isInventoryPanelActive || isPausePanelActive)
            global::PlayerDisable.instance.DisablePlayer();
        else
            global::PlayerDisable.instance.EnablePlayer();
    }

    public void SetNotePanelActive()
    {
        if (isPausePanelActive) return;

        NoteManagerUI.instance?.ToggleCurrentNote();
        PlayerDisable();
    }

    public void SetInventoryPanelActive(bool setInventoryPanel)
    {
        if (isPausePanelActive) return;

        isInventoryPanelActive = setInventoryPanel;
        inventoryPanel.SetActive(setInventoryPanel);

        ManageCursorVisibility(setInventoryPanel);
        PlayerDisable();

        if (!setInventoryPanel)
            ContextMenu.HideContextMenu();
    }

    public void SetPausePanelActive(bool setPausePanel)
    {
        isPausePanelActive = setPausePanel;
        pauseMenuPanel.SetActive(setPausePanel);
        ManageCursorVisibility(setPausePanel);

        if (setPausePanel)
        {
            wasInventoryPanelActiveBeforePause = isInventoryPanelActive;

            if (isInventoryPanelActive)
            {
                isInventoryPanelActive = false;
                inventoryPanel.SetActive(false);
                ContextMenu.HideContextMenu();
            }
        }
        else
        {
            if (wasInventoryPanelActiveBeforePause)
            {
                isInventoryPanelActive = true;
                inventoryPanel.SetActive(true);
            }
        }

        PlayerDisable();
    }

    private void ManageCursorVisibility(bool isActive)
    {
        Cursor.visible = isActive;
        Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
    }
}

