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

    private readonly Stack<BaseScreen> _screens = new ();

    [Inject]
    private void Construct(ICameraService cameraService)
    {
      _cameraService = cameraService;
    }
        
    Canvas.StaticCanvas IGuiService.StaticCanvas => _staticCanvas;
        

    void IGuiService.Push(BaseScreen screen)
    {
      if (_screens.TryPeek(out BaseScreen oldScreen))
      {
        oldScreen.SetActive(false);
      }
            
      _screens.Push(screen);
    }

    void IGuiService.Pop()
    {
      if (_screens.TryPop(out BaseScreen oldScreen))
      {
        Destroy(oldScreen.gameObject);
      }
            
      if (_screens.TryPeek(out BaseScreen screen))
      {
        screen.SetActive(true);
      }
    }

    void IGuiService.Cleanup()
    {
      foreach (BaseScreen screen in _screens)
      {
        Destroy(screen.gameObject);
      }
            
      _screens.Clear();
    }
  }
}