using UnityEngine;

public class NoteInteraction : InteractiveObject
{
    public string noteSound;
    public GameObject notePanel;

    public override void Interact(GameObject player = null)
    {
        if (notePanel != null)
        {
            NoteManagerUI.instance.ToggleNote(notePanel);

            if (notePanel.activeSelf && AudioManager.instance != null && !string.IsNullOrEmpty(noteSound))
                AudioManager.instance.Play(noteSound);
        }
    }
}


