using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Botonesopcion2 : MonoBehaviour
{
    public string Nivel;
    public void CargarEscena(string scene)
    {
        SceneManager.LoadScene(scene);
        Cursor.visible = false;
        Cursor.visible = true;
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }
    public void ClickToMenu()
    {
        SceneManager.LoadScene(0);
        SceneManager.LoadScene(Nivel);
    }
}
