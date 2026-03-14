using _Project.Scripts.Infrastructure.Gui.Screens;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Visual.UI.Menus.GameMenu
{
    public class GameMenuWindow : BaseScreen
    {
        [SerializeField] private GameObject _menuPanel;
        [SerializeField] private GameObject _controlsPanel;

        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _controlsButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _closeControlsButton;
    
        [SerializeField] private GameObject _startBlackScreen;
        private SceneLoaderService _sceneLoaderService;
        private IMenuActionsService _menuActionsService;
        private ICursorService _cursorService;
        private bool _isPaused;
    
        [Inject]
        public void Construct(
            ICursorService cursorService,
            SceneLoaderService sceneLoaderService,
            IMenuActionsService menuActionsService)
        {
            _cursorService = cursorService;
            _sceneLoaderService = sceneLoaderService;
            _menuActionsService = menuActionsService;
        
            _pauseButton.onClick.AddListener(TogglePause);
            _controlsButton.onClick.AddListener(ToggleControls);
            _closeControlsButton.onClick.AddListener(ToggleCloseControls);
            _resumeButton.onClick.AddListener(TogglePause);
        
            _exitButton.onClick.AddListener(_menuActionsService.ExitGame);
            _saveButton.onClick.AddListener(_menuActionsService.SaveGame);
            _loadButton.onClick.AddListener(_menuActionsService.LoadGame);
            _restartButton.onClick.AddListener(_menuActionsService.RestartLevel);
        }

    
    
    
        private void ToggleCloseControls()
        {
            SetControlsVisibility(false);
            SetStartScreenVisible(false);
            SetPause(false);
        }
        private void ToggleControls()
        {
            SetControlsVisibility(true);
        }
        private void TogglePause()
        {
            SetPause(!_isPaused);
        }
        public override async UniTask Show()
        {
            Initialize();
            SetStartScreenVisible(false);
        
        }
        public void SetStartScreenVisible(bool isVisible)
        {
            if (_startBlackScreen != null)
                _startBlackScreen.SetActive(isVisible);
        }
        public void Initialize()
        {
            SetMenuVisiblity(false);
            SetControlsVisibility(false);
        }

        public void SetMenuVisiblity(bool isMenuVisible)
        {
            _menuPanel.SetActive(isMenuVisible);
            _pauseButton.gameObject.SetActive(!isMenuVisible);
        }
        public void SetControlsVisibility(bool isVisible)
        {
            _controlsPanel.SetActive(isVisible);
            if (isVisible) _menuPanel.SetActive(false); 
        }
        private void SetPause(bool isPaused)
        {
            _isPaused = isPaused;

            Time.timeScale = isPaused ? 0f : 1f;

            SetMenuVisiblity(isPaused);

            if (isPaused)
            {
                _cursorService.SetDefaultCursor();
                _cursorService.SetVisible(true);
                _cursorService.SetLockState(false); 
            }
            else
            {
                _cursorService.SetCrosshairCursor();
                _cursorService.SetVisible(true);
                _cursorService.SetLockState(false); 
            }
        }
    
        public override ScreenType GetScreenType()
        {
            return ScreenType.InGameWindow;
        }
    }
}