using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Gui.Service
{
  public sealed class GuiService : MonoBehaviour, IGuiService
  {
    [SerializeField] private Canvas.StaticCanvas _staticCanvas;
        
    private readonly Stack<BaseScreen> _screens = new ();

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
        screen.Show().Forget();
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