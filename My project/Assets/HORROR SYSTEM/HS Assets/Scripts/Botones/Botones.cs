using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;

public class Botones : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Successfully exited the game :D");
    }

    public void nextScene()
    {
        SceneManager.LoadScene(1);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("The game has loaded successfully! 0 w 0");
    }
    public void Menu()
    {
        SceneManager.LoadScene(0);
        AudioListener.pause = false;
        Time.timeScale = 1;
        Debug.Log("And back to the start we go!");
    }
}
