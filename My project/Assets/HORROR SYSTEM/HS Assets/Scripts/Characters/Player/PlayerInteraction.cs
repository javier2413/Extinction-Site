using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public string interactableTag = "Interactable";

    [Header("UI")]
    public GameObject interactionPanel; // Fixed panel on screen

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

        // Update current object and panel visibility
        currentObject = closest != null ? closest.gameObject : null;
        if (interactionPanel != null)
            interactionPanel.SetActive(currentObject != null);
    }

    void HandleInteractionInput()
    {
        if (currentObject != null && Input.GetKeyDown(KeyCode.E))
        {
            var interactive = currentObject.GetComponent<InteractiveObject>();
            if (interactive != null)
            {
                interactive.Interact(gameObject);

                // If it's a note, make sure it sets the panel content
                var note = currentObject.GetComponent<NoteInteraction>();
                if (note != null)
                {
                    NoteManagerUI.instance.ToggleNote(note.notePanel);
                    // or UIManager.instance.SetNotePanelActive(note.noteData);
                }
            }
        }
    }

    // Keep this empty for now so PlayerController can still call it
    public void HandleInteract() { }
}




