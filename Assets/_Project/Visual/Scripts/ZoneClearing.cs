using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ZoneClearing : MonoBehaviour
{
    [SerializeField] private CombatZone _finalZone;
    [SerializeField] private GameObject _victoryObject;
    

    private void Start()
    {
        
        // OpenGates();
        _finalZone.OnZoneCleared
            .Take(1)
            .Subscribe(_ => _victoryObject.SetActive(true))
            .AddTo(this);
        
    }

    
    
}
