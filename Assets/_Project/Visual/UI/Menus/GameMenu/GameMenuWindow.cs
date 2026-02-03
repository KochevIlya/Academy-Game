using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class GameMenuWindow : MonoBehaviour
{
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _resumeButton;

    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _exitButton;

    public IObservable<Unit> OnPauseClicked => _pauseButton.OnClickAsObservable();
    public IObservable<Unit> OnResumeClicked => _resumeButton.OnClickAsObservable();
    public IObservable<Unit> OnRestartClicked => _restartButton.OnClickAsObservable();
    public IObservable<Unit> OnExitClicked => _exitButton.OnClickAsObservable();
    
    public void Initialize()
    {
        SetMenuVisiblity(false);
    }

    public void SetMenuVisiblity(bool isMenuVisible)
    {
        _menuPanel.SetActive(isMenuVisible);
        _pauseButton.gameObject.SetActive(!isMenuVisible);
    }
}