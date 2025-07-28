using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{

    public Animator playerAnim;
    void Start()
    {
        // Ensure the player is sitting at the start
        playerAnim.SetBool("Sitting", true);
    }

    public void OnClikcStartButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnClickExitButton()
    {
        Application.Quit();
    }
}
