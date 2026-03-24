using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.SaveLoad;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Scenes.Game.Infrastructure.States;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SaveProgressState : IEnterState
{
    private readonly ISaveLoadService _saveLoadService;

    public SaveProgressState(
        ISaveLoadService saveLoadService
        )
    {
        _saveLoadService = saveLoadService;
    }

    public UniTask Enter(IGameStateMachine gameStateMachine)
    {
        _saveLoadService.Save();
        
        gameStateMachine.Enter<GameLoopState>();
        return UniTask.CompletedTask;
    }
}
