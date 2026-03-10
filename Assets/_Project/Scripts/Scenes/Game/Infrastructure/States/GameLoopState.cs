using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Infrastructure.States
{
  public class GameLoopState : IEnterState
  {
    private readonly IGuiService _guiService;
    private readonly ICursorService _cursorService;

    public GameLoopState(IGuiService guiService, ICursorService cursorService)
    {
      _guiService = guiService;
      _cursorService = cursorService;
    }
    public UniTask Enter(IGameStateMachine gameStateMachine)
    {
      Time.timeScale = 1f;
      _guiService.ShowInGameWindow();
      return UniTask.CompletedTask;
    }
  }
}