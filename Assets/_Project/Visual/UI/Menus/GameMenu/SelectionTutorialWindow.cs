using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

public class SelectionTutorialWindow : BaseScreen
{
    private HackingService _hackingService;
    private IGuiGameService _guiService;
    [SerializeField] private SkipPanel _skipHandler;
    [SerializeField] private GameObject _tutorPanel;
 
    public override async UniTask Show()
    {
        Time.timeScale = 0f;
        await base.Show();
    }
    
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

        if (_skipHandler != null)
        {
            _skipHandler.OnSkipPressed
                .Subscribe(_ => HideTutor())
                .AddTo(LifeTimeDisposable);
        }
        
        _guiService.CloseScreen(ScreenType.HackingSelectionWindow);
    }
    
    private void SwitchWindow()
    {
        Time.timeScale = 1f;
        _guiService.ShowWindow(ScreenType.AbilitiesTutorialWindow).Forget();
        _guiService.CloseScreen(ScreenType.SelectionTutorialWindow).Forget();
    }

    private void HideTutor()
    {
        Time.timeScale = 1f;
        _tutorPanel.SetActive(false);
    }
    
    public override ScreenType GetScreenType()
    {
        return ScreenType.SelectionTutorialWindow;
    }
}
