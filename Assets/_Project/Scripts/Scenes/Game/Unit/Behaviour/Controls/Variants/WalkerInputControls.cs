using System;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Hacking.Terminal;
using UniRx;
using UnityEngine;

public class WalkerInputControls : IInputControls
{
    private readonly GameUnit _self;
    private readonly HackingTerminal _terminal;
    
    private const float StopDistance = 1f;
    private readonly Transform _camTransform;
    
    public Vector3 TargetPosition => _terminal.transform.position;
    public WalkerInputControls(GameUnit self, HackingTerminal terminal)
    {
        Debug.Log("WalkerInputControls ctor");
        _self = self;
        _terminal = terminal;
        _camTransform = Camera.main.transform;
    }
    public Vector2 MousePosition
    {
        get
        {
            if (_terminal == null) return Vector2.zero;

            Vector3 targetCenter = _terminal.transform.position + Vector3.up * 1.0f;

            Vector3 screenPoint = Camera.main.WorldToScreenPoint(targetCenter);
            
            return new Vector2(screenPoint.x, screenPoint.y);
        }
    }

    public IObservable<Vector3> OnMovement => Observable.EveryUpdate().Select(_ =>
    {
        if (_terminal == null) return _self.transform.position;

        Vector3 targetPos = _terminal.transform.position;
        Vector3 selfPos = _self.transform.position;

        float distance = Vector2.Distance(
            new Vector2(targetPos.x, targetPos.z), 
            new Vector2(selfPos.x, selfPos.z)
        );

        if (distance <= StopDistance)
        {
            return selfPos;
        }

        return targetPos;
    });
    public IObservable<Vector2> OnRawMovement { get; } = Observable.Never<Vector2>();
    public IObservable<UniRx.Unit> OnShoot => Observable.Never<Unit>();
    public IObservable<UniRx.Unit> OnAbilityUse => Observable.Never<Unit>();
}
