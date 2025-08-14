using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class InventoryItem : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerClickHandler
{
    [Header("Item Settings")]
    public string itemId;
    public string itemDescription;
    public Sprite itemSprite;

    [Header("Amount Settings")]
    public TMP_Text amountText;

    [Header("Context Menu")]
    public GameObject contextMenuPrefab; // Asigna tu prefab aquí

    private CanvasGroup canvasGroup;
    private Vector2 offset;
    private Transform originalParent;

    private InteractiveItem interactiveItem;
    private ItemDatabase.Item databaseItem;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    private static GameObject currentContextMenu;
    private static InventoryItem currentItemWithMenu;

    public void SetDatabaseItem(ItemDatabase.Item item) => databaseItem = item;
    public ItemDatabase.Item GetDatabaseItem() => databaseItem;
    public void SetInteractiveItem(InteractiveItem item) => interactiveItem = item;
    public InteractiveItem GetInteractiveItem() => interactiveItem;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) Debug.LogError("CanvasGroup missing on InventoryItem");

        amountText = GetComponentInChildren<TMP_Text>();
        UpdateAmountText();
    }

    private void Update()
    {
        UpdateAmountText();
    }

    public void UpdateAmountText()
    {
        if (amountText != null && interactiveItem != null)
        {
            amountText.text = interactiveItem.GetStringCount();
            amountText.gameObject.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InventorySystem.instance != null)
            InventorySystem.instance.UpdateItemDescription(itemDescription ?? string.Empty);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (InventorySystem.instance != null)
            InventorySystem.instance.UpdateItemDescription(string.Empty);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        offset = eventData.position - (Vector2)transform.position;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position - offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        Transform targetSlot = FindFreeTargetSlot();
        if (targetSlot == null)
            transform.SetParent(originalParent);
        else
            transform.SetParent(targetSlot);
        transform.localPosition = Vector3.zero;
        transform.SetAsLastSibling();
    }

    private Transform FindFreeTargetSlot()
    {
        var pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("InventorySlot") &&
                !InventorySystem.instance.IsSlotOccupied(result.gameObject))
            {
                return result.gameObject.transform;
            }
        }
        return null;
    }

    // ===== CLIC DERECHO PARA CONTEXT MENU =====
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (currentContextMenu != null)
            {
                Destroy(currentContextMenu);
                currentContextMenu = null;
                currentItemWithMenu = null;
            }
            else if (contextMenuPrefab != null)
            {
                currentContextMenu = Instantiate(contextMenuPrefab, transform.root);
                currentContextMenu.transform.position = transform.position;
                currentContextMenu.transform.SetParent(transform.root, false);
                currentItemWithMenu = this;

                // Configura los botones del menú
                Button useButton = currentContextMenu.transform.Find("UseButton").GetComponent<Button>();
                Button dropButton = currentContextMenu.transform.Find("DropButton").GetComponent<Button>();

                useButton.onClick.AddListener(() => {
                    interactiveItem?.UseInInventory();
                    Destroy(currentContextMenu);
                    currentContextMenu = null;
                    currentItemWithMenu = null;
                });

                dropButton.onClick.AddListener(() => {
                    interactiveItem?.DropFromInventory();
                    Destroy(currentContextMenu);
                    currentContextMenu = null;
                    currentItemWithMenu = null;
                });
            }
        }
    }
}
