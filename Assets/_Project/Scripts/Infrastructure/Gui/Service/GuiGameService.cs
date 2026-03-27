using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Visual.UI.Menus.BattleMenu;
using _Project.Visual.UI.Menus.GameMenu;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
namespace _Project.Scripts.Infrastructure.Gui.Service
{
  [Serializable]
  public struct ScreenPrefabMapping
  {
    public ScreenType ScreenType;
    public BaseScreen Prefab;
  }
  public sealed class GuiGameService : MonoBehaviour, IGuiGameService
  {
    [SerializeField] private Canvas.StaticCanvas _staticCanvas;
    [SerializeField] private PauseButtonWindow _pauseButtonWindowPrefab;
    [SerializeField] private BattleScreen _battleScreenPrefab;
    [SerializeField] private ControlsWindow _controlsWindowPrefab;
    [SerializeField] private GameOverScreen _gameOverMenuWindowPrefab;
    [SerializeField] private SaveWindow _saveWindowPrefab;
    [SerializeField] private HackingSelectionWindow _hackingSelectionWindowPrefab;
    [SerializeField] private HackingWindow _hackingwindowPrefab;
    
    [SerializeField] private List<ScreenPrefabMapping> _screenMappings;
    private Dictionary<ScreenType, BaseScreen> _prefabsDictionary;
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
      _prefabsDictionary = new Dictionary<ScreenType, BaseScreen>();
      foreach (var mapping in _screenMappings)
      {
        if (!_prefabsDictionary.ContainsKey(mapping.ScreenType))
        {
          _prefabsDictionary.Add(mapping.ScreenType, mapping.Prefab);
        }
        else
        {
          Debug.LogWarning($"[GuiGameService] Дубликат типа экрана в настройках: {mapping.ScreenType}");
        }
      }
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

    public void ShowGameOver() => ShowScreen(_gameOverMenuWindowPrefab);

    public void ShowPauseMenuWindow() => ShowScreen(pauseMenuWindowPrefab).Forget();

    public void ShowBattleScreen() => ShowScreen(_battleScreenPrefab).Forget();
    public void ShowPauseButton() => ShowScreen(_pauseButtonWindowPrefab).Forget();
    public void ShowControlsWindow() => ShowScreen(_controlsWindowPrefab).Forget();
    public void ShowMainMenuWindow(bool isAlreadySaved) => _guiService.ShowMainMenuWindow(isAlreadySaved);
    public void ShowSaveMenuWindow() => ShowScreen(_saveWindowPrefab).Forget(); 
    
    public void ShowHackingSelectionWindow() => ShowScreen(_hackingSelectionWindowPrefab).Forget();
    

    public async UniTask<BaseScreen> ShowWindow(ScreenType type)
    {
      if (_prefabsDictionary == null) 
      {
        Debug.LogError("[GuiGameService] Словарь префабов не инициализирован! Метод Construct был вызван?");
        return null;
      }
      if (!_prefabsDictionary.TryGetValue(type, out var prefab))
      {
        Debug.LogError($"[GuiGameService] Префаб для экрана {type} не найден в словаре! Добавь его в инспекторе.");
        return null;
      }

      var screenInstance = _container.InstantiatePrefabForComponent<BaseScreen>(prefab);

      ((IGuiGameService)this).Push(screenInstance);

      screenInstance.transform.SetParent(_staticCanvas.Canvas.transform, false);

      try
      {
        await screenInstance.Show();
      }
      catch (OperationCanceledException)
      {
        Debug.Log($"[GuiGameService] Показ экрана {type} отменён");
      }

      return screenInstance;
    }
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

    public async UniTask ShowHackingWindow()
    {
      await ShowScreen(_hackingwindowPrefab);
    }



    void IGuiGameService.Pop()
    {
      if (!_screens.TryPop(out var closedScreen)) return;

      bool wasOverlay = closedScreen.IsOverlay;
      Destroy(closedScreen.gameObject);

      if (wasOverlay)
      {
        RefreshVisibility();
      }
    }

    UniTask IGuiGameService.Cleanup()
    {
      foreach (var screen in _screens)
      {
        Destroy(screen.gameObject);
      }

      _screens.Clear();
      return UniTask.CompletedTask;
    }
  }
}