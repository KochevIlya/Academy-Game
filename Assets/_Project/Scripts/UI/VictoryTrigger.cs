using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;
using Zenject;


public class VictoryTrigger : MonoBehaviour
{
    [Inject] IGameStateMachine _gameStateMachine;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            Debug.Log("<color=yellow>ИГРОК ПОКИНУЛ КОМПЛЕКС. ПОБЕДА!</color>");
            TriggerVictory();
        }
    }

    private void TriggerVictory()
    {
        _gameStateMachine.Enter<VictoryState>();
    }
}