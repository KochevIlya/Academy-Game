using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private IProgressService _progressService;
    
    [Inject]
    public void Construct(
        IGuiService guiService,
        IMenuActionsService menuActionsService, 
        ICursorService cursorService
        ,IProgressService progressService
    )
    {
        _guiService = guiService;
        _menuActionsService = menuActionsService;
        _progressService = progressService;
            
        _continueButton.onClick.AddListener(_menuActionsService.LoadGame);
        _controlsButton.onClick.AddListener(OpenControls);
        _exitButton.onClick.AddListener(_menuActionsService.ExitGame);
        _newGameButton.onClick.AddListener(_menuActionsService.LoadNewGame);
        
    }

    public override async UniTask Show()
    {
        if(!_progressService.HasSaveFile())
            _continueButton.interactable = false;
        else
            _continueButton.interactable = true;
        
        await base.Show();
        
        Time.timeScale = 0f;
    }
    void OnEnable() 
    {
        EventSystem.current.SetSelectedGameObject(null);
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
