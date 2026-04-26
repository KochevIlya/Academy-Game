using UnityEngine;
using UniRx;

public class GateController : MonoBehaviour
{
    [SerializeField] private CombatZone _finalZone;
    [SerializeField] private Animator _gateAnimator;
    [SerializeField] private Collider _victoryTrigger;

    private void Start()
    {
        if (_victoryTrigger != null) 
            _victoryTrigger.enabled = false;

        // OpenGates();
        _finalZone.OnZoneCleared
            .Take(1)
            .Subscribe(_ => OpenGates())
            .AddTo(this);
    }

    private void OpenGates()
    {
        Debug.Log("Ворота открываются!");
        
        if (_gateAnimator != null)
            _gateAnimator.SetTrigger("Open");
        
        if (_victoryTrigger != null) 
            _victoryTrigger.enabled = true;
    }
}