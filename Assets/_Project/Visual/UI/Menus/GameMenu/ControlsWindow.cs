using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ControlsWindow : BaseScreen
{
    public override bool IsOverlay => true;
    [SerializeField] private Button _exitButton;
    
    private IGuiService _guiService;
    private UserInputControls _inputControls;
    
    [Inject]
    public void Construct(IGuiService guiService, UserInputControls inputControls)
    {
        _guiService = guiService;
        _inputControls = inputControls;

        _exitButton.onClick.AddListener(BackToMenu);
    }
    
    public override async UniTask Show()
    {
        _inputControls.OnCancel
            .Subscribe(_ => BackToMenu())
            .AddTo(this);
    }

    private void BackToMenu()
    {
        _guiService.Pop();
    }
    
    
    public override ScreenType GetScreenType()
    {
        return ScreenType.ControlsWindow;
    }
}
