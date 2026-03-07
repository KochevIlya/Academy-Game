using System;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Data;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class PatrolInputControls : IInputControls
{
    private readonly GameUnit _self;
    private readonly List<PatrolPath.PatrolPoint> _waypoints;
    
    private int _currentIndex = 0;
    private const float StopDistance = 0.5f;
    
    private bool _isWaiting = false;
    
    private const float RotationSpeed = 0.5f;
    private const float RotationRadius = 0.8f; 
    public MoverType RequiredMoverType => MoverType.Bot;
    public PatrolInputControls(GameUnit self)
    {
        _self = self;
        if (self.PatrolPath == null)
        {
            Debug.Log("watafe");
            _waypoints = new List<PatrolPath.PatrolPoint>();
            return;
        }
        _waypoints = self.PatrolPath.GetPatrolPoints();
    }

    public Vector2 MousePosition
    {
        get
        {
            Vector3 targetCenter = GetLookTarget() + Vector3.up * 1.0f;

            Vector3 screenPoint = Camera.main.WorldToScreenPoint(targetCenter);
            
            return new Vector2(screenPoint.x, screenPoint.y);
        }
    }
    
    public Vector3 GetLookTarget()
    {
        if (_waypoints == null || _waypoints.Count == 0)
            return _self.transform.position + _self.transform.forward;
        Vector3 baseTarget = _waypoints[_currentIndex].PointTransform.position;

        if (_isWaiting)
        {
            float offsetX = Mathf.Sin(Time.time * RotationSpeed) * RotationRadius;
            float offsetZ = Mathf.Cos(Time.time * RotationSpeed) * RotationRadius;
            
            return baseTarget + new Vector3(offsetX, 0, offsetZ);
        }
        
        return _waypoints[_currentIndex].PointTransform.position;
    }
    
    public IObservable<Vector3> OnMovement => Observable.EveryUpdate().Select(_ =>
    {
        
        if (_waypoints == null || _waypoints.Count == 0 || _isWaiting) return _self.transform.position;
    
        Vector3 targetPos = _waypoints[_currentIndex].PointTransform.position;
        Vector3 myPos = _self.transform.position;
    
        float distance = Vector2.Distance(
            new Vector2(targetPos.x, targetPos.z), 
            new Vector2(myPos.x, myPos.z)
        );
        
        if (distance <= StopDistance)
        {
            if (!_isWaiting) WaitAtPoint().Forget();
            return myPos; 
        }
        
        return targetPos;
    });
    
    private async UniTaskVoid WaitAtPoint()
    {
        _isWaiting = true;
        
        float waitTime = _waypoints[_currentIndex].WaitTime;
        
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        
        _currentIndex = (_currentIndex + 1) % _waypoints.Count;
        _isWaiting = false;
    }
    public IObservable<Vector2> OnRawMovement => OnRawMovement;
    public float GetMovementSpeed(UnitStatsData stats)
    {
        return stats.patrolSpeed;
    }

    public IObservable<Unit> OnShoot => Observable.Never<Unit>();
    public IObservable<Unit> OnAbilityUse => Observable.Never<Unit>();
    
}
