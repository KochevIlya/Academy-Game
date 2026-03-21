using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuWindow : BaseScreen
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _controlsButton;
    [SerializeField] private Button _authorsButton;
    [SerializeField] private Button _exitButton;
    
    private IGuiService _guiService;
    private IMenuActionsService _menuActionsService;
    private UserInputControls _inputControls;
    private ICursorService _cursorService;
    
    [Inject]
    public void Construct(
        IGuiService guiService,
        IMenuActionsService menuActionsService,
        UserInputControls inputControls,
        ICursorService cursorService
    )
    {
        _guiService = guiService;
        _menuActionsService = menuActionsService;
        _inputControls = inputControls;
        _cursorService = cursorService;
            
        _continueButton.onClick.AddListener(_menuActionsService.LoadGame);
        _controlsButton.onClick.AddListener(OpenControls);
        _exitButton.onClick.AddListener(_menuActionsService.ExitGame);
        _newGameButton.onClick.AddListener(_menuActionsService.RestartLevel);
        
    }

    public override async UniTask Show()
    {
        await base.Show();
        Time.timeScale = 0f;
    }
    
    
    private void OpenControls()
    {
        _guiService.ShowControlsWindow(); 
    }
    
    
    public override ScreenType GetScreenType()
    {
        return ScreenType.MainMenuWindow;
    }
}
