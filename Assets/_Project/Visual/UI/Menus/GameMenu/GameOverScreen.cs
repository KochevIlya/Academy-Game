using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _Project.Scripts.Infrastructure.Gui.Screens;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : BaseScreen
{
    
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _exitButton;

    private void Awake()
    {
        _restartButton.onClick.AddListener(OnRestartClicked);
        _exitButton.onClick.AddListener(OnExitClicked);
    }   

    private void OnRestartClicked()
    {
        Debug.Log("Restart clicked");
    }

    public async UniTask Show(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
    
        await UniTask.Delay(1000, cancellationToken: token);
        base.Show().Forget();
    }
    
    private void OnExitClicked()
    {
        Time.timeScale = 0f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public override ScreenType GetScreenType() => ScreenType.GameOver; 
    
}
