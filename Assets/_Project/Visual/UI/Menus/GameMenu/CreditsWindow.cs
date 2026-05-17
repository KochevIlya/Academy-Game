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
    private float _canvasHeight;
    
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
        Canvas.ForceUpdateCanvases();
        Canvas canvas = GetComponentInParent<Canvas>();
        
        
        
        if (canvas != null)
        {
            _canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
        }
        else
        {
            _canvasHeight = 1080f; 
        }
        float movingPartHeight = _movingPart.rect.height;
        
        _movingPosY = -_canvasHeight - movingPartHeight / 2 + movingPartHeight / 4;
        _startMovingPosY = _movingPosY;
        _movingPart.anchoredPosition = new Vector2(_movingPart.anchoredPosition.x, _movingPosY);
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_movingPart);
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