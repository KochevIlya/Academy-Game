using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.SaveLoad;
using _Project.Scripts.Infrastructure.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class MenuActionsService : IMenuActionsService
{
    private readonly ISaveLoadService _saveLoadService;
    private readonly SignalBus _signalBus;
    private readonly SceneLoaderService _sceneLoaderService;
    private readonly IGameStateMachine _stateMachine;
    public MenuActionsService(
        ISaveLoadService saveLoadService, 
        SignalBus signalBus, 
        SceneLoaderService sceneLoaderService,
        IGameStateMachine stateMachine
        )
    {
        _saveLoadService = saveLoadService;
        _signalBus = signalBus;
        _sceneLoaderService = sceneLoaderService;
        _stateMachine = stateMachine;
    }

    public void SaveGame() => _saveLoadService.Save();
    
    public void LoadGame() => _stateMachine.Enter<LoadProgressState>();

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
