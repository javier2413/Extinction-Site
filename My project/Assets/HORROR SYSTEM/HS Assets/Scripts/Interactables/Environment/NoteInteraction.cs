using UnityEngine;

public class NoteInteraction : InteractiveObject
{
    [Header("Note Settings")]
    public GameObject notePanel; // Drag the scene panel here
    public string noteSound;     // Optional sound when opening the note

    public override void Interact(GameObject player = null)
    {
        // Play the note sound if assigned
        if (!string.IsNullOrEmpty(noteSound) && AudioManager.instance != null)
        {
            AudioManager.instance.Play(noteSound);
        }

        // Toggle the panel
        if (notePanel != null && NoteManagerUI.instance != null)
        {
            bool isActive = notePanel.activeSelf;
            notePanel.SetActive(!isActive);
            NoteManagerUI.instance.SetCurrentNote(!isActive ? notePanel : null);
        }
    }
}





