using System;
using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using UniRx;
using UnityEngine;
using Zenject;

public class AggroMeleeInputControls : IInputControls
{
    private readonly GameUnit _self;
    private readonly GameUnit _target;
    
    private const float StopDistance = 1f;
    private const float CallDown = 0.5f; 
    private  ICameraService _cameraService;
    public AggroMeleeInputControls(GameUnit self, GameUnit target, ICameraService cameraService)
    {
        _self = self;
        _target = target;
        _cameraService = cameraService;
    }

    public Vector3 TargetPosition => _target != null ? _target.transform.position : Vector3.zero;

    public Vector2 MousePosition
    {
        get
        {
            if (_target == null) return Vector2.zero;
            
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(_target.transform.position);
            return new Vector2(screenPoint.x, screenPoint.y);
        }
    }

    public IObservable<Vector3> OnMovement => Observable.EveryUpdate()
        .Select(_ => CalculateMeleeMovement());

    public IObservable<Vector2> OnRawMovement => Observable.Never<Vector2>();

    public IObservable<UniRx.Unit> OnShoot => Observable
        .Interval(TimeSpan.FromSeconds(CallDown))
        .Select(_ => UniRx.Unit.Default);
        
    public IObservable<Unit> OnAbilityUse => Observable.Never<Unit>();

    private Vector3 CalculateMeleeMovement()
    {
        if (_target == null) 
        {
            Debug.LogWarning("Target or Self is null!");
            return _self.transform.position;
        }
        
        Vector3 targetPos = _target.transform.position;
        Vector3 myPos = _self.transform.position;
        
        float distance = Vector2.Distance(new Vector2(targetPos.x, targetPos.z), new Vector2(myPos.x, myPos.z));

        if (distance <= StopDistance)
        {
            return myPos;
        }
    
        return targetPos;
    }
}