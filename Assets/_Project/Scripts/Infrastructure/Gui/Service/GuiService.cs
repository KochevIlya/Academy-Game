using System;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.Gui.Service
{
  public sealed class GuiService : MonoBehaviour, IGuiService
  {
    [SerializeField] private Canvas.StaticCanvas _staticCanvas;
    [SerializeField] private GameOverScreen _gameOverScreenPrefab;
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
      if (_screens.TryPeek(out var oldScreen))
      {
        oldScreen.SetActive(false);
      }

      _screens.Push(screen);
    }

    public async void ShowGameOver()
    {
      
      var screenInstance = Instantiate(_gameOverScreenPrefab);
        
      _container.InjectGameObject(screenInstance.gameObject);
      ((IGuiService)this).Push(screenInstance);
        
      screenInstance.transform.SetParent(_staticCanvas.Canvas.transform, false);

      try
      {
        await screenInstance.Show(screenInstance.GetCancellationTokenOnDestroy());
      }
      catch (OperationCanceledException)
      {
        Debug.Log("Показ экрана отменён из-за уничтожения объекта");
      }

    }
    void IGuiService.Pop()
    {
      if (_screens.TryPop(out var oldScreen))
      {
        Destroy(oldScreen.gameObject);
      }

      if (_screens.TryPeek(out var screen))
      {
        screen.Show().Forget();
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
  }
}