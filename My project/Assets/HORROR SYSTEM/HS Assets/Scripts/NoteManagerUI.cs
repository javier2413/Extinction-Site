using UnityEngine;

public class NoteManagerUI : MonoBehaviour
{
    public static NoteManagerUI instance;

    private GameObject currentNotePanel;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Open a note panel safely
    public void OpenNote(GameObject notePanel)
    {
        // Close previous note if different
        if (currentNotePanel != null && currentNotePanel != notePanel)
            currentNotePanel.SetActive(false);

        // Open the requested note
        notePanel.SetActive(true);
        currentNotePanel = notePanel;
    }
    public void SetCurrentNote(GameObject panel)
{
    currentNotePanel = panel;
}

    // Close current note
    public void CloseCurrentNote()
    {
        if (currentNotePanel != null)
        {
            currentNotePanel.SetActive(false);
            currentNotePanel = null;
        }
    }

    // Toggle current note (optional)
    public void ToggleCurrentNote()
    {
        if (currentNotePanel != null)
            CloseCurrentNote();
    }

    public bool IsNoteOpen()
    {
        return currentNotePanel != null;
    }
}
