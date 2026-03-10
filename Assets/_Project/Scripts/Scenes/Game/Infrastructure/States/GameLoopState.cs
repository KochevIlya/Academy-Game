using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.SaveLoad;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Infrastructure.States
{
  public class GameLoopState : IEnterState
  {
    private readonly IGuiService _guiService;
    private readonly ISaveLoadService _saveLoadService;
    private readonly ICursorService _cursorService;

    public GameLoopState(IGuiService guiService,
      ICursorService cursorService,
      ISaveLoadService saveLoadService
      )
    {
      _guiService = guiService;
      _cursorService = cursorService;
      _saveLoadService = saveLoadService;
    }
    public UniTask Enter(IGameStateMachine gameStateMachine)
    {
      
      if (!_saveLoadService.HasSaveFile()) 
      {
        gameStateMachine.Enter<SaveProgressState>();
      }
      Time.timeScale = 1f;
      _guiService.Cleanup();
      _guiService.ShowInGameWindow();
      return UniTask.CompletedTask;
    }
  }
}