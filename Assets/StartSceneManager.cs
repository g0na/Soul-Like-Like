using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    public void OnClikcStartButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnClickExitButton()
    {
        Application.Quit();
    }
}
