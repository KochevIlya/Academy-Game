using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.SaveLoad;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States;
using UnityEngine;
using Zenject;

public class ButtonScript : MonoBehaviour
{
    [Inject] private IMenuActionsService _actions;
    
    public void OnRestart() => _actions.RestartLevel();
    public void OnSave() => _actions.SaveGame();
    public void OnLoad() => _actions.LoadGame();
    
    
}
