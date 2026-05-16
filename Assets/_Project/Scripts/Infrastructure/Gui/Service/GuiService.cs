using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Visual.UI.Menus.BattleMenu;
using _Project.Visual.UI.Menus.GameMenu;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace _Project.Scripts.Infrastructure.Gui.Service
{
  public sealed class GuiService : MonoBehaviour, IGuiService
  {
    [SerializeField] private Canvas.StaticCanvas _staticCanvas;
    [SerializeField] private ControlsWindow _controlsWindowPrefab;
    [SerializeField] private CreditsWindow _creditsWindowPrefab;
    [SerializeField] private MainMenuWindow _mainMenuWindowPrefab;
    [SerializeField] private GreetingWindow _greetingWindowPrefab;
    [SerializeField] private BackgroundWindow _backgroundPrefab;
    private readonly Stack<BaseScreen> _screens = new Stack<BaseScreen>();
    private DiContainer _container;
    Canvas.StaticCanvas IGuiService.StaticCanvas => _staticCanvas;
    
    [Inject]
    public void Construct(DiContainer container)
    {
      _container = container;
    }
    void IGuiService.Push(BaseScreen screen)
    {
      if (screen.IsOverlay)
      {
        foreach (var oldScreen in _screens)
        {
          if(ScreenType.Background  == oldScreen.GetScreenType())
            continue;
          if (oldScreen != null)
          {
            oldScreen.gameObject.SetActive(false);
          }
        }
      }
      
      _screens.Push(screen);
    }

    public void ShowGreetingWindow()
    {
      ShowScreen(_greetingWindowPrefab).Forget();
    }

    public void CloseGreetingWindow()
    {
      CloseScreen(_greetingWindowPrefab).Forget();
    }

    public void ShowGameOver()
    {
      // ShowScreen(_gameOverScreenPrefab).Forget();
    }

    public void ShowPauseMenuWindow()
    {
      // ShowScreen(_gameMenuWindowPrefab).Forget();
    }

    public void ShowPauseButton()
    {
      // ShowScreen(_pauseButtonWindowPrefab).Forget();
    }

    public void ShowControlsWindow()
    {
      ShowScreen(_controlsWindowPrefab).Forget();
    }
    
    public void ShowCreditsWindow()
    {
      ShowScreen(_creditsWindowPrefab).Forget();
    }

    public void ShowBackground()
    {
      ShowScreen(_backgroundPrefab).Forget();
    }

    public void CloseBackground()
    {
      CloseScreen(_backgroundPrefab).Forget();
    }

    public void ShowMainMenuWindow(bool isAlreadySaved) => ShowScreen(_mainMenuWindowPrefab).Forget();
    

    private async UniTask<T> ShowScreen<T>(T prefab) where T : BaseScreen
    {
      var screenInstance = Instantiate(prefab);
    
      _container.InjectGameObject(screenInstance.gameObject);
    
      ((IGuiService)this).Push(screenInstance);
    
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
    
    
    void IGuiService.Pop()
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

    void IGuiService.Cleanup()
    {
      foreach (var screen in _screens)
      {
        Destroy(screen.gameObject);
      }

      _screens.Clear();
    }
    public async UniTask CloseScreen(ScreenType screenType)
    {
      BaseScreen screenToClose = _screens.LastOrDefault(s => s.GetScreenType() == screenType);

      if (screenToClose == null)
      {
        Debug.LogWarning($"[GuiGameService] Попытка закрыть {screenType}, но такое окно не найдено в стеке.");
        return;
      }
      
      bool wasOverlay = screenToClose.IsOverlay;
      var tempStack = _screens.ToList();
      
      _screens.Clear();
      foreach (var screen in tempStack)
      {
        if (screen == screenToClose) continue;
        _screens.Push(screen);
      }

      if (screenToClose.gameObject != null)
      {
        Destroy(screenToClose.gameObject);
      }

      if (wasOverlay)
      {
        RefreshVisibility();
      }
      await UniTask.Yield();
    }
    public async UniTask CloseScreen(BaseScreen screen)
    {
      if (screen == null || !_screens.Contains(screen)) return;

      var tempStack = new List<BaseScreen>(_screens);
      _screens.Clear();

      bool wasOverlay = screen.IsOverlay;

      for (int i = tempStack.Count - 1; i >= 0; i--)
      {
        if (tempStack[i] == screen) continue;
        _screens.Push(tempStack[i]);
      }

      Destroy(screen.gameObject);

      if (wasOverlay)
      {
        RefreshVisibility();
      }
    }
    
    private void RefreshVisibility()
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
}