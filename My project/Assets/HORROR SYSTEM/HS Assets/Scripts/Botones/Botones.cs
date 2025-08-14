using UnityEngine;
using UnityEngine.SceneManagement;

public class Botones : MonoBehaviour
{
    void Start() { }

    void Update() { }

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

    public void Retry()
    {
        SceneManager.LoadScene(1);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        Debug.Log("Se ha reiniciado el juego en la escena 1!");
    }
}

