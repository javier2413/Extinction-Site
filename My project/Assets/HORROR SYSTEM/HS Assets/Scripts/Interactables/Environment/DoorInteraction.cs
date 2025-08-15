using UnityEngine;

public class DoorInteraction : InteractiveObject
{
    [Header("Door Settings")]
    public Animator doorAnimator;

    [Space]
    public bool needKey = false;
    public string keyId; // the required key's ID

    [Space]
    public string doorLockedSound;
    public string doorOpenSound;
    public string doorCloseSound;

    private bool isDoorOpen = false;
    private bool keyUsed = false; // has the key been spent?

    public override void Interact(GameObject player = null)
    {
        var playerInventory = InventorySystem.instance;

        // If door requires a key and it hasn't been used
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
            keyUsed = true; // mark key as spent
            playerInventory.SpendSingleItem(keyId); // remove key from inventory
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

        string soundToPlay = isDoorOpen ? doorOpenSound : doorCloseSound;
        if (!string.IsNullOrEmpty(soundToPlay) && AudioManager.instance != null)
        {
            AudioManager.instance.Play(soundToPlay);
        }
    }

    private void Locked()
    {
        if (doorAnimator != null)
            doorAnimator.SetTrigger("DoorLocked");

        if (!string.IsNullOrEmpty(doorLockedSound) && AudioManager.instance != null)
            AudioManager.instance.Play(doorLockedSound);
    }
}



