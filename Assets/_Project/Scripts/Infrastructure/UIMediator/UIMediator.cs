using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.UIMediator;
using _Project.Scripts.Scenes.Game.Unit;
using UniRx;
using UnityEngine;
using Zenject;

public class UIMediator : IUIMediator
{
    private readonly HackingService _hackingService;
    // private readonly IGuiService _guiService;
    private readonly IGuiGameService _guiGameService;
    private readonly IPlayerProvider _playerProvider;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    private readonly IGameStateMachine _stateMachine; 
    private readonly IProgressService _progressService;
    public UIMediator(
        HackingService hackingService, 
        // IGuiService guiService,
        IGuiGameService guiGameService,
        IPlayerProvider playerProvider
        ,IGameStateMachine gameStateMachine
        ,IProgressService progressService
        )
    {
        _hackingService = hackingService;
        _stateMachine = gameStateMachine;
        // _guiService = guiService;
        _playerProvider = playerProvider;
        _guiGameService = guiGameService;
        _progressService = progressService;
    }

    public void Initialize()
    {
        _hackingService.IsBattleActive
            .Subscribe(active => 
            {
                if (active) ShowBattleScreen();
                else HideBattleScreen();
            })
            .AddTo(_disposables);
    }

    public void LoadGameFromPause()
    {
        _progressService.Load();
        _stateMachine.Enter<LoadProgressState>();
    }

    private void ShowBattleScreen()
    {
        _guiGameService.ShowBattleScreen();
    }

    private void HideBattleScreen()
    {
        _guiGameService.Pop();
    }
    

    public void Dispose() => _disposables.Dispose();
}