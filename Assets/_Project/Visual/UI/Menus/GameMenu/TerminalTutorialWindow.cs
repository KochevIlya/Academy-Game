using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public class TerminalTutorialWindow : BaseScreen
{

    private HackingService _hackingService;
    private IGuiGameService _guiService;
    
    
    [Inject]
    public void Construct(
        HackingService hackingService
        ,IGuiGameService guiService
    )
    {
        _hackingService = hackingService;
        _guiService = guiService;
    }
    private void Start()
    {
        _hackingService.OnHackingProcessStarted
            .Subscribe(_ => SwitchWindow())
            .AddTo(LifeTimeDisposable);
    }

    private void SwitchWindow()
    {
        _guiService.CloseScreen(ScreenType.TutorialTerminalWindow).Forget();
    }
    
    public override ScreenType GetScreenType()
    {
        return ScreenType.TutorialTerminalWindow;
    }
}
