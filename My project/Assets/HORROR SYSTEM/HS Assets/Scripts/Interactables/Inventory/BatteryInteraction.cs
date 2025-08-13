using UnityEngine;

public class BatteryInteraction : InteractiveItem
{
    private InventorySystem playerInventory;
    private FlashlightSystem playerFlashlight;

    // Amount of battery restored per battery item use (set this in Inspector or constructor)
    public float rechargeAmount = 5f;

    public override void Interact(GameObject player = null)
    {
        base.Interact(player);

        if (player == null)
        {
            Debug.LogWarning("BatteryInteraction.Interact called with null player");
            return;
        }

        this.playerInventory = player.GetComponent<InventorySystem>();
        this.playerFlashlight = player.GetComponentInChildren<FlashlightSystem>();

        var itemHasBeenAdded = playerInventory.AddItem(itemId, count, this);
        if (itemHasBeenAdded)
        {
            AudioManager.instance.Play(pickUpSound);
            Destroy(gameObject);
        }
        else
        {
            FailurePickUp();
        }
    }


    public override void UseInInventory()
    {
        if (playerInventory == null)
        {
            Debug.LogWarning("Player inventory is null in UseInInventory");
            return;
        }

        var flashlightItems = playerInventory.FindItemByType(ItemDatabase.Type.FLASHLIGHT);
        if (flashlightItems != null && flashlightItems.Count > 0 && playerFlashlight != null)
        {
            // Check battery level before recharging
            if (playerFlashlight.currentBattery < playerFlashlight.maxBattery)
            {
                playerFlashlight.RechargeIntensity(rechargeAmount);
                playerInventory.SpendSingleItem(itemId);
                flashlightItems[0].UpdateAmountText();
            }
            else
            {
                Debug.Log("Flashlight battery already full");
            }
        }
        else
        {
            Debug.Log("Flashlight is missing from inventory or playerFlashlight reference is null");
        }
    }

    public override void DropFromInventory()
    {
        if (playerInventory != null)
        {
            playerInventory.DropItems(itemId, 1);
        }
    }

    public override string GetStringCount()
    {
        return count.ToString();
    }
}

