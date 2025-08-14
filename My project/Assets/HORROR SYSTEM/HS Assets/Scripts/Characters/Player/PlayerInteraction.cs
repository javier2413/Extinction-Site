using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 1.2f;
    public Image interactionIcon;
    public TMP_Text interactionText;
    public LayerMask obstacleLayer;

    private GameObject currentObject;
    private Transform interactionPoint;
    private RectTransform iconRectTransform;
    private float interactionPointHeight = 1f;

    void Start()
    {
        interactionPoint = new GameObject("InteractionPoint").transform;
        interactionPoint.SetParent(transform);
        interactionPoint.localPosition = new Vector3(0f, interactionPointHeight, 0f);

        interactionIcon.gameObject.SetActive(false);
        iconRectTransform = interactionIcon.GetComponent<RectTransform>();
        interactionText.text = null;
    }

    void Update()
    {
        HandleInteractionCheck();
        UpdateIconPosition();
    }

    void HandleInteractionCheck()
    {
        Collider[] hits = Physics.OverlapSphere(interactionPoint.position, interactionDistance);
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (Collider col in hits)
        {
            var interactable = col.GetComponent<InteractiveObject>();
            if (interactable == null) continue;

            float dist = Vector3.Distance(interactionPoint.position, col.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = col.transform;
            }
        }

        SetCurrentObject(closest != null ? closest.gameObject : null);

        if (closest != null && Input.GetKeyDown(KeyCode.E))
        {
            closest.GetComponent<InteractiveObject>()?.Interact(gameObject);
        }
    }

    void SetCurrentObject(GameObject obj)
    {
        currentObject = obj;

        // Si hay un panel de nota abierto, no mostrar icono
        if (NoteManagerUI.instance.IsNoteOpen())
        {
            interactionIcon.gameObject.SetActive(false);
            interactionText.text = null;
            return;
        }

        interactionIcon.gameObject.SetActive(currentObject != null);
        interactionText.text = currentObject != null ? "Interact (E)" : null;
    }

    void UpdateIconPosition()
    {
        if (currentObject == null || interactionIcon == null || iconRectTransform == null || Camera.main == null)
            return;

        Transform iconPoint = currentObject.transform.Find("IconPoint");
        if (iconPoint == null) return;

        iconRectTransform.position = Camera.main.WorldToScreenPoint(iconPoint.position);
    }
}

