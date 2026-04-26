using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Scenes.Game.Unit;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class VictoryState: IEnterState
{
    private readonly IGuiGameService _guiService;
    private readonly ICursorService _cursorService;
    private readonly HackingService _hackingService;
    public VictoryState(IGuiGameService guiService,
        ICursorService cursorService,
        HackingService hackingService
    )
    {
        _guiService = guiService;
        _cursorService = cursorService;
        _hackingService = hackingService;
    }
    public async UniTask Enter(IGameStateMachine gameStateMachine)
    {
        await _guiService.Cleanup();
        
        _hackingService.RequestCancel();
        _hackingService.StopHacking();
        await _hackingService.WaitUntilFinished();
        _guiService.ShowWindow(ScreenType.VictoryWindow);
        
        _cursorService.SetDefaultCursor();
        _cursorService.SetVisible(true);
        _cursorService.SetLockState(false);
        
        Debug.Log("In GameOverState");
        Time.timeScale = 0f;
            
    }
    public void Exit()
    {
        Time.timeScale = 1f;
    }
    
}