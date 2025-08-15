using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public string interactableTag = "Interactable";

    [Header("UI")]
    public GameObject interactionPanel; // Fixed "Press E" panel

    private GameObject currentObject;
    private Transform interactionPoint;
    private float interactionPointHeight = 1f;

    void Start()
    {
        // Create a point in front of the player for distance checks
        interactionPoint = new GameObject("InteractionPoint").transform;
        interactionPoint.SetParent(transform);
        interactionPoint.localPosition = new Vector3(0f, interactionPointHeight, 0f);

        if (interactionPanel != null)
            interactionPanel.SetActive(false);
    }

    void Update()
    {
        CheckForNearbyInteractables();
        HandleInteractionInput();
        HandleInteractionPanelVisibility();
    }

    void CheckForNearbyInteractables()
    {
        Collider[] hits = Physics.OverlapSphere(interactionPoint.position, interactionDistance);
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (Collider col in hits)
        {
            if (!col.CompareTag(interactableTag))
                continue;

            float dist = Vector3.Distance(interactionPoint.position, col.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = col.transform;
            }
        }

        currentObject = closest != null ? closest.gameObject : null;
    }

    void HandleInteractionInput()
    {
        if (currentObject != null && Input.GetKeyDown(KeyCode.E))
        {
            var interactive = currentObject.GetComponent<InteractiveObject>();
            if (interactive != null)
            {
                interactive.Interact(gameObject); // Toggle the note panel

                // Hide the floating "Press E" panel immediately
                if (interactionPanel != null)
                    interactionPanel.SetActive(false);
            }
        }
    }

    void HandleInteractionPanelVisibility()
    {
        if (interactionPanel == null)
            return;

        // Only show the panel if near an object AND no note is currently open
        bool showPanel = currentObject != null;

        if (NoteManagerUI.instance != null && NoteManagerUI.instance.IsNoteOpen())
            showPanel = false;

        interactionPanel.SetActive(showPanel);
    }

    public void HandleInteract() { } // Empty for compatibility
}




