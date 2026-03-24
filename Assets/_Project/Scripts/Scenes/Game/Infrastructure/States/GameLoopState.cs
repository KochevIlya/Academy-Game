using _Project.Scripts.Infrastructure.Gui.Camera;
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
    private readonly IGuiGameService _guiGameService;
    private readonly ISaveLoadService _saveLoadService;
    private readonly ICameraService _cameraService;
    private readonly ICursorService _cursorService;
    private readonly IGuiService _guiService;

    public GameLoopState(IGuiGameService guiGameService,
      IGuiService guiService,
      ICursorService cursorService,
      ISaveLoadService saveLoadService,
      ICameraService cameraService
      )
    {
      _guiGameService = guiGameService;
      _cursorService = cursorService;
      _saveLoadService = saveLoadService;
      _cameraService = cameraService;
      _guiService = guiService;
    }
    public UniTask Enter(IGameStateMachine gameStateMachine)
    {
      Debug.Log("In GameLoopState");
      if (!_saveLoadService.HasSaveFile()) 
      {
        Debug.LogWarning("No save file found");
        gameStateMachine.Enter<SaveProgressState>();
      }
      Time.timeScale = 1f;
      _cameraService.ResetZoom();
      _guiService.Cleanup();
      _guiGameService.Cleanup();
      _guiGameService.ShowPauseButton();
      
      return UniTask.CompletedTask;
    }
  }
}