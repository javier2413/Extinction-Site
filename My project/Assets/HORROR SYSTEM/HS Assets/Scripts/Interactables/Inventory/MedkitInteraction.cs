using UnityEngine;

public class MedkitInteraction : InteractiveItem
{
    public int healAmount = 50; // Cantidad de vida que cura el medkit

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
            Destroy(gameObject); // Destruye el medkit en la escena
        }
        else
        {
            FailurePickUp();
        }
    }

    public override void UseInInventory()
    {
        // Buscar al jugador en la escena
        var player = GameObject.FindWithTag("Player");
        if (player == null) return;

        var playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        // Solo usar si no tiene la salud completa
        if (playerHealth.currentHealth < playerHealth.maxHealth)
        {
            playerHealth.Heal(healAmount);
            InventorySystem.instance.SpendSingleItem(itemId);
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



