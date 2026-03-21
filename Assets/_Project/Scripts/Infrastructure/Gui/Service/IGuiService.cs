using _Project.Scripts.Infrastructure.Gui.Screens;

namespace _Project.Scripts.Infrastructure.Gui.Service
{
  public interface IGuiService
  {
    Canvas.StaticCanvas StaticCanvas { get; }
    void Push(BaseScreen screen);
    void Pop();
    void Cleanup();
    void ShowGameOver();
    void ShowPauseMenuWindow();
    void ShowBattleScreen();
    void ShowPauseButton();
    void ShowControlsWindow();
    void ShowMainMenuWindow(bool isAlreadySaved = true);
  }
}