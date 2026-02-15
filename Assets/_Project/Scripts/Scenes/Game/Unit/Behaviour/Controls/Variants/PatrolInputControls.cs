using System;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using UniRx;
using UnityEngine;

public class PatrolInputControls : IInputControls
{
    private readonly GameUnit _self;
    private readonly Vector3[] _waypoints;
    
    private int _currentIndex = 0;
    private const float StopDistance = 0.5f;
    
    public PatrolInputControls(GameUnit self)
    {
        _self = self;
        if (self.PatrolPath == null) Debug.Log("watafe");
        _waypoints = self.PatrolPath.GetPatrolPointsPositions().ToArray();
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
        if (_waypoints == null || _waypoints.Length == 0)
            return _self.transform.position + _self.transform.forward;

        return _waypoints[_currentIndex];
    }
    
    public IObservable<Vector2> OnMovement => Observable.EveryUpdate().Select(_ =>
    {
        
        if (_waypoints == null || _waypoints.Length == 0) return Vector2.zero;
        
        Vector3 targetPos = _waypoints[_currentIndex];
        Vector3 myPos = _self.transform.position;
        
        Vector3 targetFlat = new Vector3(targetPos.x, 0, targetPos.z);
        Vector3 myFlat = new Vector3(myPos.x, 0, myPos.z);
        
        if (Vector3.Distance(myFlat, targetFlat) <= StopDistance)
        {
            _currentIndex = (_currentIndex + 1) % _waypoints.Length;
        }
        
        targetFlat = new Vector3(_waypoints[_currentIndex].x, 0, _waypoints[_currentIndex].z);
        
        Vector3 worldDirection = (targetFlat - myFlat).normalized;
        
        Transform camTransform = Camera.main.transform;
        
        Vector3 camForward = Vector3.ProjectOnPlane(camTransform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(camTransform.right, Vector3.up).normalized;
        
        float moveY = Vector3.Dot(worldDirection, camForward);
        float moveX = Vector3.Dot(worldDirection, camRight); 

        return new Vector2(moveX, moveY);
    });

    public IObservable<Vector2> OnRawMovement => OnRawMovement;
    
    public IObservable<Unit> OnShoot => Observable.Never<Unit>();
    public IObservable<Unit> OnAbilityUse => Observable.Never<Unit>();
    
}
