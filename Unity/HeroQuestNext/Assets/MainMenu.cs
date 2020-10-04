using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public enum Scenes
    {
        MainMenuScene,
        MainGameScene,
    };
    public void PlayGame ()
    {
        SceneManager.LoadScene(Scenes.MainGameScene.ToString());
        Debug.Log("Play Clicked, Switching Scene to MainGameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Clicked, Quitting");
    }
}
