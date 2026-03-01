using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Scenes.Game.Unit;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameOverState : IEnterState
{
    private readonly IGuiService _guiService;
    private readonly ICursorService _cursorService;
    public GameOverState(IGuiService guiService, ICursorService cursorService)
    {
        _guiService = guiService;
        _cursorService = cursorService;
    }
    public UniTask Enter(IGameStateMachine gameStateMachine)
    {
        _cursorService.SetDefaultCursor();
        _cursorService.SetVisible(true);
        _cursorService.SetLockState(false);
        
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
