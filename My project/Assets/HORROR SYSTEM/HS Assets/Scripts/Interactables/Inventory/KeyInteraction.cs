using UnityEngine;

public class KeyInteraction : InteractiveItem
{
    public override void Interact(GameObject player = null)
    {
        base.Interact(player);

        var playerInventory = InventorySystem.instance;
        if (playerInventory == null)
        {
            Debug.LogError("InventorySystem instance not found!");
            return;
        }

        // Force-add the key to inventory
        bool itemAdded = playerInventory.AddItem(itemId, count, this);

        if (itemAdded)
        {
            Debug.Log("Key picked up: " + itemId);
            AudioManager.instance.Play(pickUpSound);
            Destroy(gameObject);
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



