using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameMenuController : IInitializable, IDisposable
{
    private readonly GameMenuWindow _view;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    private readonly ICursorService _cursorService;
    private readonly SceneLoaderService _sceneLoaderService;
    private bool _isPaused = false;
    
    public GameMenuController(GameMenuWindow view, SceneLoaderService sceneLoaderService, ICursorService cursorService)
    {
        _view = view;
        _sceneLoaderService = sceneLoaderService;
        _cursorService = cursorService;
    }
    public void Initialize()
    {
        _view.Initialize();
        SetPause(false);
        _view.OnPauseClicked.Subscribe(_ => SetPause(true)).AddTo(_disposables);
        _view.OnResumeClicked.Subscribe(_ => SetPause(false)).AddTo(_disposables);
        _view.OnRestartClicked.Subscribe(_ => RestartGame()).AddTo(_disposables);
        _view.OnExitClicked.Subscribe(_ => ExitGame()).AddTo(_disposables);
        
    }
    private void TogglePause()
    {
        SetPause(!_isPaused);
    }
    private void SetPause(bool isPaused)
    {
        _isPaused = isPaused;

        Time.timeScale = isPaused ? 0f : 1f;

        _view.SetMenuVisiblity(isPaused);

        if (isPaused)
        {
            _cursorService.SetDefaultCursor();
            _cursorService.SetVisible(true);
            _cursorService.SetLockState(false); 
        }
        else
        {
            _cursorService.SetCrosshairCursor();
            _cursorService.SetVisible(true);
            _cursorService.SetLockState(false); 
        }
    }
    private void RestartGame()
    {
        Time.timeScale = 1f;
        _sceneLoaderService.RestartCurrentScene();
    }

    private void ExitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    public void Dispose() => _disposables.Dispose();
}