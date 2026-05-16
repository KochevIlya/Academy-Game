using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.StateMachine;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GreetingWindow : BaseScreen
{
    protected IGuiService _guiService;
    protected IGameStateMachine _gameStateMachine;
    [SerializeField] private Button _exitButton;
    
    [Inject]
    public void Construct(IGuiService guiService
    ,IGameStateMachine  gameStateMachine
    )
    { 
        _gameStateMachine = gameStateMachine;
        _guiService = guiService;
        
        _exitButton.onClick.AddListener( () =>
        {
            CloseWindow();
        });
        _exitButton.onClick.AddListener(Interract);
    }
    
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            CloseWindow();
        }
    }
    
    private void CloseWindow()
    {
        _gameStateMachine.Enter<MainMenuState>();
        _guiService.CloseScreen(ScreenType.GreetingWindow);
    }
    
    public override ScreenType GetScreenType()
    {
        return ScreenType.GreetingWindow;
    }
}
