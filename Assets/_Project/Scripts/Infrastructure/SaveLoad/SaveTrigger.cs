using _Project.Scripts.Infrastructure.SaveLoad;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;
using Zenject;

public class SaveTrigger : MonoBehaviour, ITerminalSaveable
{
    [Inject] private SignalBus _signalBus;
    [Inject] private ISaveLoadService _saveLoadService;
    private string _id;
    private bool _hasSaved = false;
    void Start()
    {
        _saveLoadService.RegisterTerminal(this);
    }
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

    public TriggerSaveData GetSaveData()
    {
        return new TriggerSaveData
        {
            HasSaved = _hasSaved,
            Id = _id
        };
    }

    public void LoadFromData(TriggerSaveData data)
    {
        _hasSaved = data.HasSaved;
    }

    public void SetId(string id)
    {
        _id = id;
    }
    private void  OnDestroy()
    {
        _saveLoadService.UnregisterTerminal(this);
    }
}