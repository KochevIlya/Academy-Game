using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.UIMediator;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace _Project.Visual.UI.Menus.GameMenu
{
    public class PauseMenuWindow : BaseScreen
    {

        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _controlsButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _loadButton;
        
        private IGuiGameService _guiService;
        private IMenuActionsService _menuActionsService;
        private UserInputControls _inputControls;
        private ICursorService _cursorService;
        private IUIMediator  _uiMediator;
        public override bool IsOverlay => true;
        
        [Inject]
        public void Construct(
            IGuiGameService guiService,
            IMenuActionsService menuActionsService,
            UserInputControls inputControls,
            ICursorService cursorService
            ,IUIMediator mediator
            )
        {
            _guiService = guiService;
            _menuActionsService = menuActionsService;
            _inputControls = inputControls;
            _cursorService = cursorService;
            _uiMediator = mediator;
            
            _resumeButton.onClick.AddListener(Resume);
            _controlsButton.onClick.AddListener(OpenControls);
            _mainMenuButton.onClick.AddListener(_menuActionsService.ExitMainMenu);
            _exitButton.onClick.AddListener(_menuActionsService.ExitGame);
            _loadButton.onClick.AddListener(_uiMediator.LoadGameFromPause);
        }
        
        
        public override async UniTask Show()
        {
            await base.Show();
            SetPause(true);
            _inputControls.OnCancel
                .Subscribe(_ => Resume())
                .AddTo(LifeTimeDisposable);
        }

        private void Resume()
        {
            SetPause(false);
            
            _guiService.Pop();
        }

        private void OpenControls()
        {
            _guiService.ShowControlsWindow(); 
        }
        protected override void OnEnable() 
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
        private void SetPause(bool isPause)
        {
            if (isPause)
            {
                _cursorService.SetDefaultCursor();
                _cursorService.SetLockState(false);
                _cursorService.SetVisible(true);
                
                Time.timeScale = 0f;
                _inputControls.IsBlocked.Value = true;
            }
            else
            {
                _cursorService.SetCrosshairCursor();
                _cursorService.SetLockState(false);
                _cursorService.SetVisible(true);
            
                Time.timeScale = 1f;
                _inputControls.IsBlocked.Value = false;
            }
            
        }
        
        
        public override ScreenType GetScreenType()
        {
            return ScreenType.InGameWindow;
        }
    }
}