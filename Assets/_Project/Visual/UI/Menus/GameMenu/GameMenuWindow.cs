using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class GameMenuWindow : MonoBehaviour
{
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _controlsPanel;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _resumeButton;

    [SerializeField] private Button _controlsButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _closeControlsButton;
    
    [SerializeField] private GameObject _startBlackScreen;

    public IObservable<Unit> OnPauseClicked => _pauseButton.OnClickAsObservable();
    public IObservable<Unit> OnResumeClicked => _resumeButton.OnClickAsObservable();
    public IObservable<Unit> OnControlsClicked => _controlsButton.OnClickAsObservable();
    public IObservable<Unit> OnExitClicked => _exitButton.OnClickAsObservable();
    public IObservable<Unit> OnCloseControlsClicked => _closeControlsButton.OnClickAsObservable();
    
    
    public void SetStartScreenVisible(bool isVisible)
    {
        if (_startBlackScreen != null)
            _startBlackScreen.SetActive(isVisible);
    }
    public void Initialize()
    {
        SetMenuVisiblity(false);
        SetControlsVisibility(false);
    }

    public void SetMenuVisiblity(bool isMenuVisible)
    {
        _menuPanel.SetActive(isMenuVisible);
        _pauseButton.gameObject.SetActive(!isMenuVisible);
    }
    public void SetControlsVisibility(bool isVisible)
    {
        _controlsPanel.SetActive(isVisible);
        if (isVisible) _menuPanel.SetActive(false); 
    }
}