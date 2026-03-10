using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;
using Zenject;

public class SaveTrigger : MonoBehaviour
{
    [SerializeField] private string _uniqueId;
    [Inject] private SignalBus _signalBus;
    private bool _hasSaved = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!_hasSaved && IsPlayer(other))
        {
            _hasSaved = true;
            _signalBus.Fire<SaveRequestedSignal>();
            
            Debug.Log("Signal Sent: SaveRequested");
        }
    }
    
    private bool IsPlayer(Collider other) => other.GetComponent<GameUnit>()?.CompareTag("Player") ?? false;
    
}