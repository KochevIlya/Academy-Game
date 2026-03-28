using System.Collections;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Visual.UI.Menus.GameMenu
{
    public class SaveWindow : BaseScreen
    {
    
        private IGuiGameService _guiService;
    
        [Inject]
        public void Construct(
            IGuiGameService guiService
        )
        {
            _guiService = guiService;
        }
    
        public override async UniTask Show()
        {
            await base.Show();
            
            await UniTask.Delay(1000);

            if (this != null && gameObject != null)
            {
                _guiService.CloseScreen(this).Forget();
            }

        }

        public override ScreenType GetScreenType()
        {
            return ScreenType.SaveWindow;
        }
    }
}
