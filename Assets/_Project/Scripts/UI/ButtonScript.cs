using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Infrastructure.StateMachine.States;
using UnityEngine;
using Zenject;

public class ButtonScript : MonoBehaviour
{
    private SignalBus _signalBus;
    
    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public void OnRestartClick()
    {
        _signalBus.Fire<RestartLevelSignal>();
    }
}
