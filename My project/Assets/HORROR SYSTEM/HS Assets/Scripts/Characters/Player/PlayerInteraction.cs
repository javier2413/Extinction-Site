using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 1.2f;

    [Header("Interaction UI")]
    public Image interactionIcon;
    public TMP_Text interactionText;

    [Header("Detection Settings")]
    public LayerMask obstacleLayer;

    private RectTransform iconRectTransform;
    private GameObject currentObject;
    private Transform interactionPoint;
    private float interactionPointHeight = 1f;

    private void Start()
    {
        interactionPoint = new GameObject("InteractionPoint").transform;
        interactionPoint.SetParent(transform);
        interactionPoint.localPosition = new Vector3(0f, interactionPointHeight, 0f);

        interactionIcon.gameObject.SetActive(false);
        iconRectTransform = interactionIcon.GetComponent<RectTransform>();
        interactionText.text = null;
    }

    private void Update()
    {
        HandleInteraction();
        IconPoint();
    }

    private void HandleInteraction()
    {
        Collider[] hitColliders = Physics.OverlapSphere(interactionPoint.position, interactionDistance);
        Transform closestObject = null;
        float minDistance = float.MaxValue;

        foreach (Collider collider in hitColliders)
        {
            var interactable = collider.GetComponent<InteractiveObject>();
            if (interactable == null) continue;

            float distance = Vector3.Distance(interactionPoint.position, collider.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestObject = collider.transform;
            }
        }

        SetCurrentObject(closestObject != null ? closestObject.gameObject : null);

        if (closestObject != null && Input.GetKeyDown(KeyCode.E))
        {
            PickUpObject(closestObject.gameObject);
        }
    }

    public void PickUpObject(GameObject objectToPick)
    {
        objectToPick.GetComponent<InteractiveObject>()?.Interact(gameObject);
    }

    private void IconPoint()
    {
        if (currentObject == null || interactionIcon == null || iconRectTransform == null || Camera.main == null)
            return;

        Transform iconPoint = currentObject.transform.Find("IconPoint");
        if (iconPoint == null)
            return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(iconPoint.position);
        iconRectTransform.position = screenPos;
    }

    public void SetCurrentObject(GameObject obj)
    {
        currentObject = obj;
        interactionIcon.gameObject.SetActive(currentObject != null);
        UpdateInteractionText();
    }

    private void UpdateInteractionText()
    {
        interactionText.text = currentObject != null ? $"Interact (E)" : null;
    }
}
