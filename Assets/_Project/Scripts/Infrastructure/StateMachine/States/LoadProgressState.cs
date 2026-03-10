using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.SaveLoad;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Scenes.Game.Infrastructure.States;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadProgressState : IEnterState
{
    private readonly ISaveLoadService _saveLoadService;

    public LoadProgressState(ISaveLoadService saveLoadService)
    {
        _saveLoadService = saveLoadService;
        
    }

    public async UniTask Enter(IGameStateMachine gameStateMachine)
    {
        await _saveLoadService.LoadAsync();
        
        gameStateMachine.Enter<GameLoopState>();
    }
}