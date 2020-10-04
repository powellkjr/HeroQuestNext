using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(MainMenu.Scenes.MainMenuScene.ToString());
        Debug.Log("Main Menu Clicked, Switching Scene to MainMenuScene");
    }
}
