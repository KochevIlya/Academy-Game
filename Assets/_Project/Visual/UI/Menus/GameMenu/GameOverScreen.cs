using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _Project.Scripts.Infrastructure.Gui.Screens;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameOverScreen : BaseScreen
{
    private IMenuActionsService _menuActionsService;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _loadButton;
    
    [Inject]
    public void Construct(IMenuActionsService menuActionsService)
    {
        _menuActionsService = menuActionsService;
        _restartButton.onClick.AddListener(_menuActionsService.RestartLevel);
        _exitButton.onClick.AddListener(_menuActionsService.ExitGame);
        _loadButton.onClick.AddListener(_menuActionsService.LoadGame);
    }

    

    public async UniTask Show(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
    
        await UniTask.Delay(1000, cancellationToken: token);
        base.Show().Forget();
    }

    public override ScreenType GetScreenType() => ScreenType.GameOver; 
    
}
