using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using _Project.Sounds;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Project.Visual.UI.Menus.GameMenu
{
    public class GameControlsWindow : ControlsWindow
    {
        private IGuiGameService _guiGameService;
        private UserInputControls _inputControls;
    
        [Inject]
        public void Construct(IGuiGameService guiGameService
            ,UserInputControls inputControls
            ,IGuiService guiService
            , ISoundService soundService
        )
        {
            base.Construct(guiService, soundService);
            _guiGameService = guiGameService;
            _inputControls = inputControls;

        }
    
        public override async UniTask Show()
        {
            await base.Show();
            _inputControls.OnCancel
                .Subscribe(_ =>
                {
                    BackToMenu();
                    _soundService.Play(Audio.AudioType.Click);
                })
                .AddTo(this);
        }
        protected override void BackToMenu()
        {
            Debug.Log($"[GameControlsWindow] Pop called at {Time.time}");
            _guiGameService.Pop();
        }
    
    }
}
