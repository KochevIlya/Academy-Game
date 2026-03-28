using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public class SelectionTutorialWindow : BaseScreen
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
        _hackingService.OnHackingStarted
            .Subscribe(_ => SwitchWindow())
            .AddTo(LifeTimeDisposable);
        _guiService.CloseScreen(ScreenType.HackingSelectionWindow);
    }
    
    private void SwitchWindow()
    {
        _guiService.ShowWindow(ScreenType.AbilitiesTutorialWindow).Forget();
        _guiService.CloseScreen(ScreenType.SelectionTutorialWindow).Forget();
    }
    
    public override ScreenType GetScreenType()
    {
        return ScreenType.SelectionTutorialWindow;
    }
}
