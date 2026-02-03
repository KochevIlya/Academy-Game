using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneLoaderService
{
    private readonly ZenjectSceneLoader _sceneLoader;

    public SceneLoaderService(ZenjectSceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }

    public void RestartCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        _sceneLoader.LoadScene(currentSceneName, LoadSceneMode.Single);
    }
}
