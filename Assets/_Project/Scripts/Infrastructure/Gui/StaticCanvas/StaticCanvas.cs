using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.Gui.StaticCanvas
{
  public sealed class GuiService : MonoBehaviour, IGuiService
  {
    [SerializeField] private Canvas.StaticCanvas _staticCanvas;

    private ICameraService _cameraService;

    private readonly Stack<BaseScreen> _screens = new Stack<BaseScreen>();

    [Inject]
    private void Construct(ICameraService cameraService)
    {
      _cameraService = cameraService;
    }

    Canvas.StaticCanvas IGuiService.StaticCanvas => _staticCanvas;


    void IGuiService.Push(BaseScreen screen)
    {
      if (_screens.TryPeek(out var oldScreen))
      {
        oldScreen.SetActive(false);
      }

      _screens.Push(screen);
    }

    void IGuiService.Pop()
    {
      if (_screens.TryPop(out var oldScreen))
      {
        Destroy(oldScreen.gameObject);
      }

      if (_screens.TryPeek(out var screen))
      {
        screen.SetActive(true);
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