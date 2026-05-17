using UnityEngine;
using UniRx;

public class GateController : MonoBehaviour
{
    [SerializeField] private CombatZone _finalZone;
    [SerializeField] private Animator _gateAnimator;
    [SerializeField] private Collider _victoryTrigger;
    [SerializeField] private TriggerEnter _openingTrigger;
    
    private bool _isActive = false;

    private void Start()
    {
        if (_victoryTrigger != null) 
            _victoryTrigger.enabled = false;

        // OpenGates();
        _finalZone.OnZoneCleared
            .Take(1)
            .Subscribe(_ => _isActive = true)
            .AddTo(this);
        
        
        
        _openingTrigger.OnTriggerEnterSubject
            .Subscribe(_ =>
                {
                    if (_isActive)
                    {
                        OpenGates();
                    }
                }
                )
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