using UnityEngine;

public class NoteInteraction : InteractiveObject
{
    public string noteSound;
    public GameObject notePanel;

    private bool isOpen = false;

    public override void Interact(GameObject player = null)
    {
        if (notePanel != null)
        {
            // Toggle the note panel on/off
            isOpen = !isOpen;
            notePanel.SetActive(isOpen);

            if (isOpen && AudioManager.instance != null && !string.IsNullOrEmpty(noteSound))
            {
                AudioManager.instance.Play(noteSound);
            }
        }
        else
        {
            Debug.LogWarning("Note panel not assigned on " + gameObject.name);
        }
    }
}

