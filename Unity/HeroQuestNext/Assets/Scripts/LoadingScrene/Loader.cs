using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum eScenesType
{
    MainMenuScene,
    MainGameScene,
    LoadingScene,
};

public static class Loader
{
    private class LoadingMonoBehavior: MonoBehaviour
    {
        //this is just a class
    }
    private static Action onLoaderCallback;
    private static AsyncOperation aLoadingAscyncOperation;
    public static void Load(eScenesType inScene)
    {
        //set the Loader Callback action to be loading the target screen
        onLoaderCallback = () =>
        {
            GameObject gLoadingGameObject = new GameObject("Loading Game Object");
            gLoadingGameObject.AddComponent<LoadingMonoBehavior>().StartCoroutine(LoadSceneAsync(inScene));

        };
        //but load the loading screen first
        SceneManager.LoadScene(eScenesType.LoadingScene.ToString());
        Debug.Log("Play Clicked, Switching Scene to MainGameScene");
    }

    public static IEnumerator LoadSceneAsync(eScenesType inScene)
    {
        yield return null;


        aLoadingAscyncOperation = SceneManager.LoadSceneAsync(inScene.ToString());

        while (!aLoadingAscyncOperation.isDone)
        {
            yield return null;

        }
    }

    public static float GetLoadingProgress()
    {
        if(aLoadingAscyncOperation!=null)
        {
            return aLoadingAscyncOperation.progress;
        }
        else
        {
            return 1f;
        }
    }
    public static void LoaderCallback()
    {
        //Execute the loader call back action will load the target screen
    
        if(onLoaderCallback !=null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }    
    }
}
