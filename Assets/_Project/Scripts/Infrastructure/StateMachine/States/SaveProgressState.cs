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
    private readonly IProgressService _progressService;
    public SaveProgressState(
        ISaveLoadService saveLoadService
        ,IProgressService progressService
        )
    {
        _progressService = progressService;
        _saveLoadService = saveLoadService;
    }

    public async UniTask Enter(IGameStateMachine gameStateMachine)
    {
        await UniTask.NextFrame();
        Debug.Log("In SaveProgreessState");
        _saveLoadService.Save();
        gameStateMachine.Enter<GameLoopState>();
    }
}
