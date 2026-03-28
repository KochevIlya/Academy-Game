using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using _Project.Visual.UI.Menus.BattleMenu;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
public class PauseButtonWindow : BaseScreen{
    [SerializeField] private Button _pauseButton;
    
    private IGuiGameService _guiService;
    private UserInputControls _inputControls;
    public override bool IsOverlay => false;
    
    [Inject]
    public void Construct(
        IGuiGameService guiService,
        UserInputControls inputControls
    )
    {
        _guiService = guiService;
        _inputControls = inputControls;
        _pauseButton.onClick.AddListener(OpenPauseMenu);
    }
    protected void Awake() {
        Debug.Log($"[UI] PauseButtonWindow AWAKE. InstanceID: {this.GetInstanceID()}");
    }
    
    public override async UniTask Show()
    {
        await base.Show();
        LifeTimeDisposable.Clear(); 
    
        _inputControls.OnCancel
            .Subscribe(_ => OpenPauseMenu())
            .AddTo(LifeTimeDisposable);
    }

    private void OpenPauseMenu()
    {
        _guiService.ShowPauseMenuWindow(); 
    }


    private void OnDestroy()
    {
        LifeTimeDisposable.Clear();
        Debug.Log($"[UI] PauseButtonWindow DESTROYED. InstanceID: {this.GetInstanceID()}");
    }

    public override ScreenType GetScreenType()
    {
        return ScreenType.PauseButtonWindow;
    }
}
