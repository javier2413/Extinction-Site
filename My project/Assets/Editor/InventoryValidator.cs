using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class InventoryValidator : EditorWindow
{
    private ItemDatabase database;

    [MenuItem("Tools/Validate Inventory Items")]
    public static void ShowWindow()
    {
        GetWindow<InventoryValidator>("Inventory Validator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Inventory Validator", EditorStyles.boldLabel);

        database = (ItemDatabase)EditorGUILayout.ObjectField("Item Database", database, typeof(ItemDatabase), false);

        if (GUILayout.Button("Validate Scene Items"))
        {
            ValidateSceneItems();
        }
    }

    private void ValidateSceneItems()
    {
        if (database == null)
        {
            Debug.LogError("No ItemDatabase assigned!");
            return;
        }

        InventoryItem[] allItems = GameObject.FindObjectsOfType<InventoryItem>();

        List<InventoryItem> invalidItems = new List<InventoryItem>();

        foreach (var item in allItems)
        {
            bool existsInDatabase = database.items.Exists(dbItem => dbItem.id == item.itemId);
            if (!existsInDatabase)
            {
                invalidItems.Add(item);
                Debug.LogWarning($"Invalid InventoryItem found: '{item.itemId}' on GameObject '{item.gameObject.name}'");
            }
        }

        Debug.Log($"Validation complete. {invalidItems.Count} invalid items found.");
    }
}
