using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.StaticData;
using Zenject;

namespace _Project.Scripts.Infrastructure.Gui.Factory
{
    public sealed class UIFactory : IUIFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IGuiService _guiService;
        private readonly DiContainer _container;

        public UIFactory(IStaticDataService staticDataService, IGuiService guiService, 
            DiContainer container)
        {
            _staticDataService = staticDataService;
            _guiService = guiService;
            _container = container;
        }

        BaseScreen IUIFactory.CreateScreen(ScreenType type, params object[] extraArgs)
        {
            // _guiService.Pop();
            // ScreenData data = _staticDataService.ScreenData(type);
            // GameObject prefab = Screen(data);
            // BaseScreen screen = _container.InstantiatePrefabForComponent<BaseScreen>(prefab, _guiService.StaticCanvas.CurrentCanvas.Canvas.transform, extraArgs);
            // _guiService.Push(screen);
            // return screen;
            
            return null;
        }
        
        BaseScreen IUIFactory.CreatePopup(ScreenType type, params object[] extraArgs)
        {
            // ScreenData data = _staticDataService.ScreenData(type);
            // GameObject prefab = Screen(data);
            // BaseScreen screen = _container.InstantiatePrefabForComponent<BaseScreen>(prefab, _guiService.StaticCanvas.CurrentCanvas.Canvas.transform, extraArgs);
            // _guiService.Push(screen);
            // return screen;
            
            return null;
        }
    }
}