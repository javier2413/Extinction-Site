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

    public void ToggleNote(GameObject notePanel)
    {
        // Close previous panel if different
        if (currentNotePanel != null && currentNotePanel != notePanel)
            currentNotePanel.SetActive(false);

        bool isActive = !notePanel.activeSelf;
        notePanel.SetActive(isActive);
        currentNotePanel = isActive ? notePanel : null;
    }

    public void ToggleCurrentNote()
    {
        if (currentNotePanel != null)
            ToggleNote(currentNotePanel);
    }

    public bool IsNoteOpen()
    {
        return currentNotePanel != null;
    }
}

