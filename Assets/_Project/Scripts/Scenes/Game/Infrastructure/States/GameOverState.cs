using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameOverState : IEnterState
{

    public UniTask Enter(IGameStateMachine gameStateMachine)
    {
        Time.timeScale = 0f;
        Debug.Log("<color=red>GAME OVER: Сознание потеряно.</color>");
            
        return UniTask.CompletedTask;
    }
    
    
}
