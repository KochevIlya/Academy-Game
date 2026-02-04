using System;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using UniRx;
using UnityEngine;

public class AggroInputControls : IInputControls
{
    private readonly GameUnit _self;
    private readonly GameUnit _target;

    public Vector3 TargetPosition => _target.transform.position;
    public AggroInputControls(GameUnit self, GameUnit target)
    {
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
    public Vector3 GetLookTarget() 
    {
        return _target.transform.position; 
    }

    public IObservable<Vector2> OnMovement => Observable.Return(Vector2.zero);
    public IObservable<Vector2> OnRawMovement => Observable.Return(Vector2.zero);

    public IObservable<UniRx.Unit> OnShoot => Observable
        .Interval(TimeSpan.FromSeconds(0.7f))
        .Select(_ => UniRx.Unit.Default);
    public IObservable<Unit> OnAbilityUse => Observable.Never<Unit>();
}