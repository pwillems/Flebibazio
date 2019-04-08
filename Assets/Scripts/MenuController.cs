using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{

    // Menu options
    public bool isEndless;
    public bool isLevels;
    public bool isHighscores;
    public bool isQuit;

    public void GotoLevelOne()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level 1");
    }

    public void GotoEndless()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Infinite");
    }

    public void GotoQuit()
    {
        Application.Quit();
    }
}
