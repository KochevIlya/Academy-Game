using _Project.Scripts.Infrastructure.Gui.Screens;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Infrastructure.Gui.Service
{
  public interface IGuiService
  {
    Canvas.StaticCanvas StaticCanvas { get; }
    void Push(BaseScreen screen);
    void Pop();
    void Cleanup();
    void ShowGreetingWindow();
    void ShowGameOver();
    void ShowPauseMenuWindow();
    void ShowPauseButton();
    void ShowControlsWindow();
    void ShowCreditsWindow();
    void ShowBackground();
    void CloseBackground();
    void ShowMainMenuWindow(bool isAlreadySaved = true);
    UniTask CloseScreen(ScreenType screenType);
    UniTask CloseScreen(BaseScreen screen);
  }
}