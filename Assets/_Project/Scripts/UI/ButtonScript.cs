using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States;
using UnityEngine;
using Zenject;

public class ButtonScript : MonoBehaviour
{
    private IGameStateMachine _stateMachine;

    [Inject]
    public void Construct(IGameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void OnRestartClick()
    {
        if (_stateMachine == null)
        {
            Debug.LogError("Ошибка: _stateMachine не инициализирована! Проверьте Zenject Binding.");
            return;
        }
        else
        {
            _stateMachine.Enter<ReloadCurrentSceneState>();
        }
    }
}
