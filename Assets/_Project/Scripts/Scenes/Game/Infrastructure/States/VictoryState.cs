using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Sounds;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class VictoryState: IEnterState
{
    private readonly IGuiGameService _guiService;
    private readonly ICursorService _cursorService;
    private readonly HackingService _hackingService;
    private readonly ISoundService _soundService;
    public VictoryState(IGuiGameService guiService,
        ICursorService cursorService,
        HackingService hackingService
        , ISoundService soundService
    )
    {
        _guiService = guiService;
        _soundService = soundService;
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
        
        _soundService.StopAll();
        _soundService.Play(Audio.AudioType.Win);
        
        Observable.Timer(TimeSpan.FromSeconds(5), Scheduler.MainThreadIgnoreTimeScale)
            .Subscribe(_ => _soundService.Play(Audio.AudioType.Global));
        
        Debug.Log("In GameOverState");
        Time.timeScale = 0f;
            
    }
    public void Exit()
    {
        Time.timeScale = 1f;
    }
    
}