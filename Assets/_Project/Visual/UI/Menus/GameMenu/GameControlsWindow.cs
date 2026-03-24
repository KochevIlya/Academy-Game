using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public class GameControlsWindow : ControlsWindow
{
    private IGuiGameService _guiGameService;
    private UserInputControls _inputControls;
    
    [Inject]
    public void Construct(IGuiGameService guiGameService
        ,UserInputControls inputControls
        ,IGuiService guiService
    )
    {
        base.Construct(guiService);
        _guiGameService = guiGameService;
        _inputControls = inputControls;

    }
    
    public override async UniTask Show()
    {
        await base.Show();
        _inputControls.OnCancel
            .Subscribe(_ => BackToMenu())
            .AddTo(this);
    }
    protected override void BackToMenu()
    {
        Debug.Log($"[GameControlsWindow] Pop called at {Time.time}");
        _guiGameService.Pop();
    }
    
}
