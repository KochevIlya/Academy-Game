using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.SaveLoad;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MainMenuState : IEnterState
{
    private readonly IGuiService _guiService;
    private readonly IProgressService _progressService;
    private readonly ICursorService _cursorService;
    public MainMenuState(IGuiService guiService,
        ICursorService cursorService
    )
    {
        _guiService = guiService;
        _cursorService = cursorService;
    }

    public UniTask Enter(IGameStateMachine gameStateMachine)
    {
        _cursorService.SetDefaultCursor();
        _cursorService.SetVisible(true);
        _cursorService.SetLockState(false);
        
        _guiService.ShowMainMenuWindow();
        
        
        return UniTask.CompletedTask;
    }
}
