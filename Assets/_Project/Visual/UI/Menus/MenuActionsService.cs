using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.SaveLoad;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class MenuActionsService : IMenuActionsService
{
    
    private readonly SignalBus _signalBus;
    private readonly IGameStateMachine _stateMachine;
    private readonly IProgressService _progressService;
    public MenuActionsService(
        // ISaveLoadService saveLoadService, 
        SignalBus signalBus, 
        IGameStateMachine stateMachine,
        IProgressService progressService
        )
    {
        // _saveLoadService = saveLoadService;
        _signalBus = signalBus;
        _stateMachine = stateMachine;
        _progressService = progressService;
    }


    public void LoadGame()
    {
        _progressService.Load();
        _stateMachine.Enter<LoadProjectState>();
    }
    
    
    public void LoadNewGame()
    {
        _progressService.Progress = null;
        _stateMachine.Enter<LoadProjectState>();
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

    public void ExitMainMenu()
    {
        _stateMachine.Enter<ExitToMainMenuState>();
    }
}
