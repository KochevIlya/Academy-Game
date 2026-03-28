using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Infrastructure.Gui.Screens;
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
    private readonly IProgressService _progressService;

    public GameLoopState(IGuiGameService guiGameService,
      IGuiService guiService,
      ICursorService cursorService,
      ISaveLoadService saveLoadService,
      ICameraService cameraService
      ,IProgressService progressService
      )
    {
      _guiGameService = guiGameService;
      _cursorService = cursorService;
      _saveLoadService = saveLoadService;
      _cameraService = cameraService;
      _guiService = guiService;
      _progressService = progressService;
    }
    public async UniTask Enter(IGameStateMachine gameStateMachine)
    {
      Debug.Log("In GameLoopState");
      if (!_saveLoadService.HasSaveFile()) 
      {
        Debug.LogWarning("No save file found");
        gameStateMachine.Enter<SaveProgressState>();
        return;
      }
      Debug.Log($"[UI] Start Cleanup. Time: {Time.frameCount}");
      _guiService.Cleanup();
      
      await _guiGameService.Cleanup();
      Debug.Log($"[UI] Cleanup finished. Waiting for frame... Time: {Time.frameCount}");
      
      if (_progressService.IsFirstStart)
      {
        Debug.Log($"[SpawnGameState] Showing TutorialMovementWindow");
        _guiGameService.ShowWindow(ScreenType.TutorialMovementWindow);
        _progressService.IsFirstStart = false;
      }
      
      await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
      Debug.Log($"[UI] Creating Pause Button. Time: {Time.frameCount}");
      
      _cameraService.ResetZoom();
      _guiGameService.ShowPauseButton();
      
      Time.timeScale = 1f;
    }
  }
}