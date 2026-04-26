using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.UIMediator;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class VictoryWindow : BaseScreen
{
    private IMenuActionsService _menuActionsService;
    private IUIMediator _uiMediator;
    
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _exitButton;

    public override bool IsOverlay => true;
    
    [Inject]
    public void Construct(IMenuActionsService menuActionsService
        ,IUIMediator  uiMediator
    )
    { 
        _menuActionsService = menuActionsService;
        _uiMediator = uiMediator;
        
        _mainMenuButton.onClick.AddListener(_menuActionsService.ExitMainMenu);
        _exitButton.onClick.AddListener(_menuActionsService.ExitGame);
    }

    

    public async UniTask Show(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
    
        await UniTask.Delay(1000, cancellationToken: token);
        base.Show().Forget();
    }

    public override ScreenType GetScreenType() => ScreenType.VictoryWindow; 
    
}