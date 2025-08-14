using System.Collections.Generic;
using UnityEngine;

// Interface for all interactable objects
public interface IInteractable
{
    void Interact(GameObject player = null);
}

public class InteractiveItem : InteractiveObject, IInteractable
{
    public string itemId;
    public int count;
    public string pickUpSound;
    public string failurePickUpSound;
    public List<AvalaibleInteraction> avalaibleInteractions = new() { AvalaibleInteraction.USE, AvalaibleInteraction.DROP };

    private ItemDatabase.Item databaseItem;

    public override void Interact(GameObject player = null)
    {
        base.Interact(player);
        // Additional item-specific interaction logic can go here
    }

    protected void Start()
    {
        var database = InventorySystem.instance.itemDatabase;
        var item = database.GetItem(itemId);
        if (item == null)
        {
            Debug.LogError("Item with ID " + itemId + " doesn't exist in Database");
        }
        databaseItem = item;
    }

    protected void FailurePickUp()
    {
        UIManager.instance.ShowInventoryFullNotification();
        AudioManager.instance.Play(failurePickUpSound);
    }

    public virtual void UseInInventory() { }

    public virtual void DropFromInventory() { }

    public virtual string GetStringCount()
    {
        return "";
    }

    public enum AvalaibleInteraction
    {
        USE,
        DROP
    }

    public ItemDatabase.Item GetDatabaseItem()
    {
        return databaseItem;
    }
}

