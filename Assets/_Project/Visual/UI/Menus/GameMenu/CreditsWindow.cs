using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.UIMediator;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CreditsWindow : BaseScreen
{
    protected IGuiService _guiService;
    [SerializeField] private RectTransform _movingPart;
    [SerializeField] private Button _mainMenuButton;

    [SerializeField] private float _movingPosY;
    [SerializeField] private float _startMovingPosY;
    
    [SerializeField] private RectTransform _lastObject;
    [SerializeField] private RectTransform _logo;

    private bool _isEnd = false;
    [SerializeField] private float _speed = 50f;
    
    public override bool IsOverlay => true;
    
    [Inject]
    public void Construct(IGuiService guiService
    )
    { 
        _guiService = guiService;
        
        _mainMenuButton.onClick.AddListener(_guiService.Pop);
        _mainMenuButton.onClick.AddListener(Interract);
    }

    private void Start()
    {
        Debug.Log(Screen.currentResolution.height);
        Debug.Log($"[CreditsWindow] MovingPart.SizeDelta.y: {_movingPart.sizeDelta.y}");
        _movingPosY = - Screen.currentResolution.height;
        Debug.Log($"[CreditsWindow] _movingPosY: {_movingPosY}");
        _startMovingPosY = - Screen.currentResolution.height;
        Debug.Log($"[CreditsWindow] _startMovingPosY: {_startMovingPosY}");
        _lastObject.sizeDelta = new Vector2(_lastObject.sizeDelta.x, _lastObject.sizeDelta.y 
            + Screen.currentResolution.height / 2 - _logo.sizeDelta.y / 2);
        
        //_movingPosY = _startMovingPosY - 300;
    }

    private void Update()
    {
        _movingPosY += _speed * Time.unscaledDeltaTime;
        
        if (_movingPosY <= -_startMovingPosY //+ _logo.GetComponent<RectTransform>().anchoredPosition.y/2)
            )
        {
            //_movingPosY = _startMovingPosY - Screen.currentResolution.height;
            _movingPart.anchoredPosition = new Vector2(_movingPart.anchoredPosition.x, _movingPosY);
        }
    }

    public override ScreenType GetScreenType() => ScreenType.CreditsWindow; 
    
}