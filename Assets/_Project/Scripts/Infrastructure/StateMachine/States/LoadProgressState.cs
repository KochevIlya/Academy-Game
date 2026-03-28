using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.PersistentProgress.Data;
using _Project.Scripts.Infrastructure.SaveLoad;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Scenes.Game.Infrastructure.States;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadProgressState : IEnterState
{
    
    private readonly ISaveLoadService _saveLoadService;
    private readonly IProgressService _progressService;

    public LoadProgressState(ISaveLoadService saveLoadService, IProgressService progressService)
    {
        _saveLoadService = saveLoadService;
        _progressService = progressService;
    }

    public async UniTask Enter(IGameStateMachine gameStateMachine)
    {
        if (_progressService.Progress != null)
        {
            Debug.Log("Progress initialized.");
            await _saveLoadService.LoadAsync();
            gameStateMachine.Enter<GameLoopState>();
        }

        else 
        {
            Debug.Log("Progress is null.");
            _progressService.Progress = new LevelData();
            gameStateMachine.Enter<SaveProgressState>().Forget();
        }
        
    }
    
}