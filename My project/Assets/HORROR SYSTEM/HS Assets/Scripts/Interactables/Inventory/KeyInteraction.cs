using UnityEngine;

public class KeyInteraction : InteractiveItem
{
    private InventorySystem playerInventory;

    public override void Interact(GameObject player = null)
    {
        base.Interact(player);

        playerInventory = player.GetComponent<InventorySystem>();

        if (playerInventory != null)
        {
            bool itemHasBeenAdded = playerInventory.AddItem(itemId, count, this);
            if (itemHasBeenAdded)
            {
                AudioManager.instance.Play(pickUpSound);
                Destroy(gameObject);  // Destroy the key object in the scene
            }
            else
            {
                FailurePickUp();
            }
        }
        else
        {
            Debug.LogWarning("Player does not have an InventorySystem component!");
        }
    }

    public override void DropFromInventory()
    {
        playerInventory.DropItems(itemId, 1);
    }
}

