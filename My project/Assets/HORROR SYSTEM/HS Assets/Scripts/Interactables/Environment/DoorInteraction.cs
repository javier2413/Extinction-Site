using UnityEngine;

public class DoorInteraction : InteractiveObject
{
    [Header("Door Settings")]
    public Animator doorAnimator;

    [Space]
    public bool needKey;
    public string keyId;

    [Space]
    public string doorLockedSound;
    public string doorOpenSound;
    public string doorCloseSound;

    private bool isDoorOpen = false;
    private bool keyUsed = false; // tracks if key has been spent

    public override void Interact(GameObject player = null)
    {
        var playerInventory = InventorySystem.instance;

        if (needKey && !keyUsed)
        {
            TryUseKey(playerInventory);
        }
        else
        {
            ToggleDoor();
        }
    }

    private void TryUseKey(InventorySystem playerInventory)
    {
        if (playerInventory == null)
        {
            Locked();
            return;
        }

        var keys = playerInventory.FindItemByType(ItemDatabase.Type.KEY);

        if (keys == null)
        {
            Locked();
            return;
        }

        // Look for the correct key
        var correctKey = keys.Find(key => keyId.Equals(key.itemId));
        if (correctKey != null)
        {
            keyUsed = true; // mark the key as used
            playerInventory.SpendSingleItem(keyId); // remove key
            ToggleDoor();
        }
        else
        {
            Locked();
        }
    }

    private void ToggleDoor()
    {
        isDoorOpen = !isDoorOpen;

        if (doorAnimator != null)
        {
            doorAnimator.SetBool("DoorOpen", isDoorOpen);
            doorAnimator.SetBool("DoorClose", !isDoorOpen);
        }

        if (isDoorOpen)
        {
            AudioManager.instance.Play(doorOpenSound);
        }
        else
        {
            AudioManager.instance.Play(doorCloseSound);
        }
    }

    private void Locked()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("DoorLocked");
        }

        AudioManager.instance.Play(doorLockedSound);
    }
}



