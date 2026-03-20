using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Visual.UI.Menus.GameMenu
{
    public class GameMenuWindow : BaseScreen
    {

        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _controlsButton;
        [SerializeField] private Button _exitButton;
    
        private IGuiService _guiService;
        private IMenuActionsService _menuActionsService;
        private UserInputControls _inputControls;
        private ICursorService _cursorService;
        public override bool IsOverlay => true;
        
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
            
            _resumeButton.onClick.AddListener(Resume);
            _controlsButton.onClick.AddListener(OpenControls);
            _restartButton.onClick.AddListener(_menuActionsService.RestartLevel);
            _exitButton.onClick.AddListener(_menuActionsService.ExitGame);
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