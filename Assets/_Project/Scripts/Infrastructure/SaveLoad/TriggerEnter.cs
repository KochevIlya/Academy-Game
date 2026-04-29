using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Unit = UniRx.Unit;

public class TriggerEnter : MonoBehaviour
{

    private readonly Subject<UniRx.Unit> _triggerEnterSubject = new Subject<UniRx.Unit>();
    public IObservable<UniRx.Unit> OnTriggerEnterSubject => _triggerEnterSubject;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            _triggerEnterSubject.OnNext(Unit.Default);
    }
}
