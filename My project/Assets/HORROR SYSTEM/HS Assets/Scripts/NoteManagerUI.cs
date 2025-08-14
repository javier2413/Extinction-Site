using System.Collections;
using System.Collections.Generic;
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
        // Cierra el panel anterior si hay uno distinto
        if (currentNotePanel != null && currentNotePanel != notePanel)
            currentNotePanel.SetActive(false);

        bool isActive = !notePanel.activeSelf;
        notePanel.SetActive(isActive);
        currentNotePanel = isActive ? notePanel : null;
    }

    public bool IsNoteOpen()
    {
        return currentNotePanel != null;
    }
}

