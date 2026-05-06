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

    public override bool IsOverlay => true;
    
    [Inject]
    public void Construct(IGuiService guiService
    )
    { 
        _guiService = guiService;
        
        _mainMenuButton.onClick.AddListener(_guiService.Pop);
    }

    private void Start()
    {
        _startMovingPosY = -_movingPart.sizeDelta.y / 2;
        _movingPosY = _startMovingPosY - 300;

    }

    private void Update()
    {
        _movingPosY += 1f;
        if (_movingPosY >= -_startMovingPosY)
            _movingPosY = _startMovingPosY - Screen.currentResolution.height;
        
        _movingPart.anchoredPosition = new Vector2(_movingPart.anchoredPosition.x, _movingPosY);
    }

    public override ScreenType GetScreenType() => ScreenType.CreditsWindow; 
    
}