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
    [SerializeField] protected Button _exitButton;
    
    protected IGuiService _guiService;
    
    [Inject]
    public virtual void Construct(IGuiService guiService
        )
    {
        _guiService = guiService;
    }
    protected virtual void Awake() 
    {
        _exitButton.onClick.AddListener(BackToMenu);
    }

    protected virtual void BackToMenu()
    {
        _guiService.Pop();
    }
    
    public override ScreenType GetScreenType()
    {
        return ScreenType.ControlsWindow;
    }
}
