using System;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Visual.UI.Menus.BattleMenu;
using _Project.Visual.UI.Menus.GameMenu;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts.Infrastructure.Gui.Service
{
  public sealed class GuiGameService : MonoBehaviour, IGuiGameService
  {
    [SerializeField] private Canvas.StaticCanvas _staticCanvas;
    [SerializeField] private PauseButtonWindow _pauseButtonWindowPrefab;
    [SerializeField] private BattleScreen _battleScreenPrefab;
    [SerializeField] private ControlsWindow _controlsWindowPrefab;
    [SerializeField] private GameOverScreen _gameOverMenuWindowPrefab;
    [SerializeField] private SaveWindow _saveWindowPrefab;
    [SerializeField] private DefaultWindow _hackingSelectionWindowPrefab;
    [SerializeField] private HackingWindow _hackingwindowPrefab;
    [FormerlySerializedAs("_gameMenuWindowPrefab")] [SerializeField] private PauseMenuWindow pauseMenuWindowPrefab;
    private readonly Stack<BaseScreen> _screens = new Stack<BaseScreen>();
    private DiContainer _container;
    Canvas.StaticCanvas IGuiGameService.StaticCanvas => _staticCanvas;
    private IGuiService _guiService;
    
    
    [Inject]
    public void Construct(DiContainer container, IGuiService guiService)
    {
      _container = container;
      _guiService = guiService;
    }

    void IGuiGameService.Push(BaseScreen screen)
    {
      if (screen.IsOverlay)
      {
        foreach (var oldScreen in _screens)
        {
          if (oldScreen != null)
          {
            oldScreen.gameObject.SetActive(false);
          }
        }
      }

      _screens.Push(screen);
    }

    public void ShowGameOver() => ShowScreen(_gameOverMenuWindowPrefab).Forget();

    public void ShowPauseMenuWindow() => ShowScreen(pauseMenuWindowPrefab).Forget();

    public void ShowBattleScreen() => ShowScreen(_battleScreenPrefab).Forget();
    public void ShowPauseButton() => ShowScreen(_pauseButtonWindowPrefab).Forget();
    public void ShowControlsWindow() => ShowScreen(_controlsWindowPrefab).Forget();
    public void ShowMainMenuWindow(bool isAlreadySaved) => _guiService.ShowMainMenuWindow(isAlreadySaved);
    public void ShowSaveMenuWindow() => ShowScreen(_saveWindowPrefab).Forget(); 
    
    public void ShowHackingSelectionWindow() => ShowScreen(_hackingSelectionWindowPrefab).Forget();
    public async UniTask ShowHackingWindow() => await ShowScreen(_hackingwindowPrefab);

    private async UniTask<T> ShowScreen<T>(T prefab) where T : BaseScreen
    {
      var screenInstance = _container.InstantiatePrefabForComponent<T>(prefab);

      ((IGuiGameService)this).Push(screenInstance);

      screenInstance.transform.SetParent(_staticCanvas.Canvas.transform, false);

      try
      {
        await screenInstance.Show();
      }
      catch (OperationCanceledException)
      {
        Debug.Log($"Показ экрана {typeof(T).Name} отменён");
      }

      return screenInstance;
    }


    void IGuiGameService.Pop()
    {
      if (!_screens.TryPop(out var closedScreen)) return;

      bool wasBlockingEverything = closedScreen.IsOverlay;
      Destroy(closedScreen.gameObject);

      if (wasBlockingEverything)
      {
        foreach (var screen in _screens)
        {
          if (screen == null) continue;

          screen.gameObject.SetActive(true);

          screen.Show().Forget();
          if (screen.IsOverlay)
          {
            break;
          }
        }
      }
    }

    void IGuiGameService.Cleanup()
    {
      foreach (var screen in _screens)
      {
        Destroy(screen.gameObject);
      }

      _screens.Clear();
    }
  }
}