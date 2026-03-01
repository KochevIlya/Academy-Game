using System;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class AggroDistanceInputControls : IInputControls
{
    private readonly GameUnit _self;
    private readonly GameUnit _target;
    
    
    private const float MinDistance = 5f; 
    private const float MaxDistance = 15f;
    private const float AimThreshold = 0.9f;
    
    private float _strafeTimer;
    private float _strafeChangeInterval = 1f;
    private float _currentStrafeDir = 0f;

    private const float CoolDown = 0.7f;
    public Vector3 TargetPosition => _target != null ? _target.transform.position : Vector3.zero;
    public AggroDistanceInputControls(GameUnit self, GameUnit target)
    {
        _self = self;
        _target = target;
        
        PickNewStrafeDirection();
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

    public IObservable<Vector2> OnMovement => Observable.EveryUpdate()
        .Select(_ => CalculateCombatMovement());
    public IObservable<Vector2> OnRawMovement => Observable.Return(Vector2.zero);

    public IObservable<UniRx.Unit> OnShoot => Observable
        .Interval(TimeSpan.FromSeconds(CoolDown))
        .Select(_ => UniRx.Unit.Default);
    public IObservable<Unit> OnAbilityUse => Observable.Never<Unit>();
    
    private Vector2 CalculateCombatMovement()
    {
        if (_target == null || _self == null) return Vector2.zero;

        Vector3 selfPos = _self.transform.position;
        Vector3 targetPos = _target.transform.position;
    
        selfPos.y = 0;
        targetPos.y = 0;

        Vector3 directionToTarget = targetPos - selfPos;
        float distance = directionToTarget.magnitude;
    
        Vector3 normalizedDirection = directionToTarget.normalized;

        float vertical = 0f;
        if (distance < MinDistance) vertical = -1f;
        else if (distance > MaxDistance) vertical = 1f;

        Vector3 finalMoveWorld = normalizedDirection * vertical;

        if (distance >= MinDistance && distance <= MaxDistance)
        {
            Vector3 playerForward = _target.transform.forward;
            Vector3 dirToBot = (_self.transform.position - _target.transform.position).normalized;
            float dotProduct = Vector3.Dot(playerForward, dirToBot);

            if (dotProduct > AimThreshold)
            {
                _strafeTimer -= Time.deltaTime;
                if (_strafeTimer <= 0)
                {
                    _currentStrafeDir = Random.value > 0.5f ? 1f : -1f;
                    _strafeTimer = _strafeChangeInterval;
                }
            
                Vector3 sideStep = new Vector3(-normalizedDirection.z, 0, normalizedDirection.x);
                finalMoveWorld += sideStep * _currentStrafeDir;
            }
        }

        Vector3 localMovement = _self.transform.InverseTransformDirection(finalMoveWorld);
    
        return new Vector2(localMovement.x, localMovement.z);
    }
    private void PickNewStrafeDirection()
    {
        _currentStrafeDir = Random.value > 0.5f ? 1f : -1f;
        
        _strafeTimer = _strafeChangeInterval;
        
    }
    
    
}