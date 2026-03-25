using System;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Data;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Utils.Extensions;
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

            Vector3 targetCenter = new Vector3(
                _target.transform.position.x,
                _target.WeaponPoint.position.y,
                _target.transform.position.z
            );

            Vector3 screenPoint = Camera.main.WorldToScreenPoint(targetCenter);
            
            return new Vector2(screenPoint.x, screenPoint.y);
        }
    }

    public IObservable<Vector3> OnMovement => Observable.EveryUpdate()
        .Select(_ => CalculateCombatMovement());
    public IObservable<Vector2> OnRawMovement => Observable.Return(Vector2.zero);
    public MoverType RequiredMoverType => MoverType.Bot;

    public float GetMovementSpeed(UnitStatsData stats)
    {
        return stats.speed;
    }

    public IObservable<UniRx.Unit> OnShoot => Observable
        .Interval(TimeSpan.FromSeconds(CoolDown))
        .Select(_ => UniRx.Unit.Default);
    public IObservable<Unit> OnAbilityUse => Observable.Never<Unit>();
    
    private Vector3 CalculateCombatMovement()
    {
        if (_target == null) return _self.transform.position;

        Vector3 selfPos = _self.transform.position;
        Vector3 targetPos = _target.transform.position;
        Vector3 dirToTarget = (targetPos - selfPos).normalized;
        float distance = Vector3.Distance(new Vector3(selfPos.x, 0, selfPos.z), new Vector3(targetPos.x, 0, targetPos.z));

        Vector3 desiredPosition = selfPos;

        if (distance < MinDistance)
        {
            desiredPosition = targetPos - dirToTarget * MinDistance;
        }
        else if (distance > MaxDistance)
        {
            desiredPosition = targetPos - dirToTarget * MaxDistance;
        }
        else
        {
            Vector3 playerForward = _target.transform.forward;
            Vector3 dirToBot = (selfPos - targetPos).normalized;
        
            if (Vector3.Dot(playerForward, dirToBot) > AimThreshold)
            {
                _strafeTimer -= Time.deltaTime;
                if (_strafeTimer <= 0)
                {
                    _currentStrafeDir = UnityEngine.Random.value > 0.5f ? 1f : -1f;
                    _strafeTimer = _strafeChangeInterval;
                }

                Vector3 sideDir = new Vector3(-dirToTarget.z, 0, dirToTarget.x);
                desiredPosition = selfPos + sideDir * _currentStrafeDir * 2f;
            }
            else
            {
                return selfPos;
            }
        }

        return desiredPosition;
    }
    private void PickNewStrafeDirection()
    {
        _currentStrafeDir = Random.value > 0.5f ? 1f : -1f;
        
        _strafeTimer = _strafeChangeInterval;
        
    }
    
    
}