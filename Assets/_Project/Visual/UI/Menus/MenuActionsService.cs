using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.SaveLoad;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class MenuActionsService : IMenuActionsService
{
    private readonly ISaveLoadService _saveLoadService;
    private readonly SignalBus _signalBus;
    private readonly SceneLoaderService _sceneLoaderService;

    public MenuActionsService(
        ISaveLoadService saveLoadService, 
        SignalBus signalBus, 
        SceneLoaderService sceneLoaderService)
    {
        _saveLoadService = saveLoadService;
        _signalBus = signalBus;
        _sceneLoaderService = sceneLoaderService;
    }

    public void SaveGame() => _saveLoadService.Save();
    
    public void LoadGame() => _saveLoadService.LoadAsync().Forget();

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        _signalBus.Fire<RestartLevelSignal>();
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
