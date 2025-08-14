using UnityEngine;

public class MedkitInteraction : InteractiveItem
{
    public override void Interact(GameObject player = null)
    {
        base.Interact(player);

        var playerInventory = InventorySystem.instance;
        if (playerInventory == null)
        {
            Debug.LogWarning("InventorySystem instance not found.");
            return;
        }

        bool itemHasBeenAdded = playerInventory.AddItem(itemId, count, this);
        if (itemHasBeenAdded)
        {
            AudioManager.instance.Play(pickUpSound);
            Destroy(gameObject); // Destroy the medkit in the scene
        }
        else
        {
            FailurePickUp();
        }
    }

    public override void UseInInventory()
    {
        // Find the player dynamically
        var player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object not found in scene.");
            return;
        }

        var playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth component not found on player.");
            return;
        }

        if (playerHealth.currentHealth < playerHealth.maxHealth)
        {
            playerHealth.SetMaxHealth(); // or Heal(amount) if you want partial healing
            InventorySystem.instance.SpendSingleItem(itemId);
            Debug.Log("Used medkit: Health restored.");
        }
        else
        {
            Debug.Log("Maximum health reached, medkit not used.");
        }
    }

    public override void DropFromInventory()
    {
        InventorySystem.instance.DropItems(itemId, 1);
    }

    public override string GetStringCount()
    {
        return count.ToString();
    }
}


