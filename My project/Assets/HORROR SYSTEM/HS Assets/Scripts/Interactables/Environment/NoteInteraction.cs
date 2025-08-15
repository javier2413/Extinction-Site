using UnityEngine;

public class NoteInteraction : InteractiveObject
{
    [Header("Note Settings")]
    public GameObject notePanel; // Assign the UI panel for this note
    public string noteSound;     // Optional sound when opening the note

    public override void Interact(GameObject player = null)
    {
        // Play the note sound
        if (!string.IsNullOrEmpty(noteSound) && AudioManager.instance != null)
        {
            AudioManager.instance.Play(noteSound);
        }

        // Toggle this note panel via the NoteManagerUI
        if (NoteManagerUI.instance != null && notePanel != null)
        {
            NoteManagerUI.instance.ToggleNote(notePanel);
        }
    }
}



