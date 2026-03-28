using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.UIMediator;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameOverScreen : BaseScreen
{
    private IMenuActionsService _menuActionsService;
    private IUIMediator _uiMediator;
    
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _loadButton;

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
        _loadButton.onClick.AddListener(_uiMediator.LoadGameFromPause);
    }

    

    public async UniTask Show(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
    
        await UniTask.Delay(1000, cancellationToken: token);
        base.Show().Forget();
    }

    public override ScreenType GetScreenType() => ScreenType.GameOver; 
    
}
