using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public class MenuManager : MonoBehaviour {

    public void StartGame()
    {
        EditorSceneManager.LoadScene("Infinity Mode");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("We quit the game!");
    }
}
