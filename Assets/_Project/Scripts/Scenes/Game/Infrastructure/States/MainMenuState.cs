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
    private readonly ISaveLoadService _saveLoadService;

    public MainMenuState(IGuiService guiService,
        ICursorService cursorService,
        ISaveLoadService saveLoadService,
        ICameraService cameraService
    )
    {
        _guiService = guiService;
        _saveLoadService = saveLoadService;
    }

    public UniTask Enter(IGameStateMachine gameStateMachine)
    {
        Time.timeScale = 0f;
        if (!_saveLoadService.HasSaveFile()) 
        {
            _guiService.ShowMainMenuWindow(false);
        }
        else
        {
            _guiService.ShowMainMenuWindow(true);
        }
        
        
        return UniTask.CompletedTask;
    }
}
