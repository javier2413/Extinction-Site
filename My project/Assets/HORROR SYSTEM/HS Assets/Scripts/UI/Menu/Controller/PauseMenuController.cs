using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PauseController : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("The panel that contains the pause menu UI.")]
    public GameObject pauseMenuPanel;

    private bool isPaused = false;

    void Update()
    {
        if (IsPausePressed())
            TogglePauseMenu();
    }

    private bool IsPausePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }

    private void TogglePauseMenu()
    {
        isPaused = !isPaused;

        // Activate parent canvas if necessary
        if (pauseMenuPanel != null && pauseMenuPanel.transform.parent != null)
        {
            var parentCanvas = pauseMenuPanel.transform.parent.gameObject;
            if (!parentCanvas.activeSelf)
                parentCanvas.SetActive(true);
        }

        // Show/hide panel
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(isPaused);

        // Freeze or resume game
        Time.timeScale = isPaused ? 0f : 1f;

        // Cursor visibility
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;

        // Enable/disable player
        var player = FindObjectOfType<PlayerController>();
        if (player != null)
            player.enabled = !isPaused;
    }
}








