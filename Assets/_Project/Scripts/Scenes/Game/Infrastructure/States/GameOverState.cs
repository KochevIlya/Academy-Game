using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameOverState : IEnterState
{
    private readonly IGuiService _guiService;
    public GameOverState(IGuiService guiService)
    {
        _guiService = guiService;
    }
    public UniTask Enter(IGameStateMachine gameStateMachine)
    {
        Time.timeScale = 0f;
        Debug.Log("In GameOverState");
        _guiService.ShowGameOver();
            
        return UniTask.CompletedTask;
    }
    public void Exit()
    {
        Time.timeScale = 1f;
    }
    
}
