using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Persistent References")]
    public GameObject interactionPanel;   // Your single UI panel
    public GameObject eventSystem;        // EventSystem object
    public GameObject player;             // Player prefab/instance
    public GameObject inventoryManager;
    public GameObject pausepanel; // Inventory manager

    void Awake()
    {
        // Handle singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Persist children
            if (interactionPanel != null) DontDestroyOnLoad(interactionPanel);
            if (eventSystem != null) DontDestroyOnLoad(eventSystem);
            if (player != null) DontDestroyOnLoad(player);
            if (inventoryManager != null) DontDestroyOnLoad(inventoryManager);

            // Subscribe to sceneLoaded to fix scene-specific references
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reassign camera for UI panel if using Screen Space - Camera
        if (interactionPanel != null)
        {
            Canvas canvas = interactionPanel.GetComponent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                canvas.worldCamera = Camera.main;
            }
        }

        // Optional: reassign EventSystem if your scene adds one
        if (eventSystem != null && !eventSystem.activeInHierarchy)
        {
            eventSystem.SetActive(true);
        }

        // Optional: reposition player if needed
        if (player != null)
        {
            // Example: reset to spawn point named "PlayerSpawn"
            Transform spawn = GameObject.Find("PlayerSpawn")?.transform;
            if (spawn != null)
                player.transform.position = spawn.position;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}


