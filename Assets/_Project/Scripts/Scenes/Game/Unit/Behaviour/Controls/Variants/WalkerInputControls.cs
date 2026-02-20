using System;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using UniRx;
using UnityEngine;

public class WalkerInputControls : IInputControls
{
    private readonly GameUnit _self;
    private readonly GameUnit _target = GameObject.FindWithTag("Player")?.GetComponent<GameUnit>();
    
    private const float StopDistance = 1f;
    private readonly Transform _camTransform;
    
    public Vector3 TargetPosition => _target.transform.position;
    public WalkerInputControls(GameUnit self)
    {
        Debug.Log("WalkerInputControls ctor");
        _self = self;
        _camTransform = Camera.main.transform;
    }
    public Vector2 MousePosition
    {
        get
        {
            if (_target == null) return Vector2.zero;

            Vector3 targetCenter = _target.transform.position + Vector3.up * 1.0f;

            Vector3 screenPoint = Camera.main.WorldToScreenPoint(targetCenter);
            
            return new Vector2(screenPoint.x, screenPoint.y);
        }
    }

    public IObservable<Vector2> OnMovement => Observable.EveryUpdate().Select(_ =>
    {
        if(_target == null) return Vector2.zero;
        
        Vector3 targetPos = _target.transform.position;
        Vector3 selfPos = _self.transform.position;
        
        Vector3 targetFlat = new Vector3(targetPos.x, 0, targetPos.z);
        Vector3 selfFlat = new Vector3(selfPos.x, 0, selfPos.z);

        if (Vector3.Distance(selfFlat, targetFlat) <= StopDistance)
        {
            Debug.Log("Цель достицнута");
            return Vector2.zero;
        }
        
        Vector3 worldDirection = (targetFlat - selfFlat).normalized;
        
        Vector3 camForward = Vector3.ProjectOnPlane(_camTransform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(_camTransform.right, Vector3.up).normalized;
        
        float moveY = Vector3.Dot(worldDirection, camForward);
        float moveX = Vector3.Dot(worldDirection, camRight);
        
        return new Vector2(moveX, moveY);
    });
    public IObservable<Vector2> OnRawMovement { get; } = Observable.Never<Vector2>();
    public IObservable<UniRx.Unit> OnShoot => Observable.Never<Unit>();
    public IObservable<UniRx.Unit> OnAbilityUse => Observable.Never<Unit>();
}
