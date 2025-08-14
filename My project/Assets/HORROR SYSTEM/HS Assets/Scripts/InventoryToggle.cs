using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel; // assign your inventory panel here

    private bool isOpen = false;

    void Update()
    {
        // Press I to toggle inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (inventoryPanel == null) return;

        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        // Freeze or resume game when inventory is open
        Time.timeScale = isOpen ? 0f : 1f;

        // Show or hide cursor
        Cursor.visible = isOpen;
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
