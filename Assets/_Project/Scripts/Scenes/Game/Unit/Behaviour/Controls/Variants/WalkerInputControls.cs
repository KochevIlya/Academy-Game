using System;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using UniRx;
using UnityEngine;

public class WalkerInputControls : IInputControls
{
    private readonly GameUnit _self;
    private readonly GameUnit _target;
    
    public Vector3 TargetPosition => _target.transform.position;
    public WalkerInputControls(GameUnit self, GameUnit target)
    {
        Debug.Log("WalkerInputControls ctor");
        _self = self;
        _target = target;
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
    public IObservable<Vector2> OnMovement { get; } = Observable.Never<Vector2>();
    public IObservable<Vector2> OnRawMovement { get; } = Observable.Never<Vector2>();
    public IObservable<UniRx.Unit> OnShoot => Observable.Never<Unit>();
    public IObservable<UniRx.Unit> OnAbilityUse => Observable.Never<Unit>();
}
