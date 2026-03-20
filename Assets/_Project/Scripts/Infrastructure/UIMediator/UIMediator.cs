using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.UIMediator;
using _Project.Scripts.Scenes.Game.Unit;
using UniRx;
using UnityEngine;
using Zenject;

public class UIMediator : IUIMediator
{
    private readonly HackingService _hackingService;
    private readonly IGuiService _guiService;
    private readonly IPlayerProvider _playerProvider;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    public UIMediator(
        HackingService hackingService, 
        IGuiService guiService,
        IPlayerProvider playerProvider
        )
    {
        _hackingService = hackingService;
        _guiService = guiService;
        _playerProvider = playerProvider;
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

    private void ShowBattleScreen()
    {
        _guiService.ShowBattleScreen();
    }

    private void HideBattleScreen()
    {
        _guiService.Pop();
    }
    

    public void Dispose() => _disposables.Dispose();
}