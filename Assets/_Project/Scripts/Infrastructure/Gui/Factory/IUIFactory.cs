using _Project.Scripts.Infrastructure.Gui.Screens;

namespace _Project.Scripts.Infrastructure.Gui.Factory
{
  public interface IUIFactory
  {
    BaseScreen CreateScreen(ScreenType type, params object[] extraArgs);
    BaseScreen CreatePopup(ScreenType type, params object[] extraArgs);
  }
}