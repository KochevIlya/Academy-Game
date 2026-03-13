using System;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit._Data;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls.Variants
{
    public class PatrolInputControls : IInputControls
    {
        private readonly GameUnit _self;
        private readonly List<PatrolPath.PatrolPoint> _waypoints;
    
        private int _currentIndex = 0;
        private const float StopDistance = 0.8f;
    
        private bool _isWaiting = false;
    
        private const float RotationSpeed = 0.1f; 
        private const float MaxViewAngle = 45f;
    
        private float _randomSeedX;
        private float _randomSeedZ;
        private float _waitStartTime;
        private Vector3 _waitForwardDirection;
    
        public MoverType RequiredMoverType => MoverType.Bot;

        public PatrolInputControls(GameUnit self)
        {
            _self = self;
            _randomSeedX = UnityEngine.Random.Range(0f, 100f);
            _randomSeedZ = UnityEngine.Random.Range(0f, 100f);

            if (self.PatrolPath == null)
            {
                _waypoints = new List<PatrolPath.PatrolPoint>();
                return;
            }
            _waypoints = self.PatrolPath.GetPatrolPoints();
        }

        public Vector2 MousePosition
        {
            get
            {
                Vector3 targetPos = GetLookTarget();
                Vector3 targetCenter = targetPos + Vector3.up * 1.5f;

                Vector3 screenPoint = Camera.main.WorldToScreenPoint(targetCenter);
                return new Vector2(screenPoint.x, screenPoint.y);
            }
        }
    
        public Vector3 GetLookTarget()
        {
            if (_waypoints == null || _waypoints.Count == 0)
                return _self.transform.position + _self.transform.forward * 5f;

            if (_isWaiting)
            {
                float yaw = (Mathf.PerlinNoise(Time.time * RotationSpeed, _randomSeedX) * 2f - 1f) * MaxViewAngle;
                float pitch = (Mathf.PerlinNoise(_randomSeedZ, Time.time * RotationSpeed) * 2f - 1f) * 10f;
            
                float blendWeight = Mathf.Clamp01((Time.time - _waitStartTime) / 1.0f);
            
                Quaternion baseRotation = Quaternion.LookRotation(_waitForwardDirection);
                Quaternion offset = Quaternion.Euler(pitch * blendWeight, yaw * blendWeight, 0);
                Vector3 lookDir = baseRotation * offset * Vector3.forward;
            
                return _self.transform.position + lookDir * 5f; 
            }
        
            return _waypoints[_currentIndex].PointTransform.position;
        }
    
        public IObservable<Vector3> OnMovement => Observable.EveryUpdate().Select(_ =>
        {
            if (_waypoints == null || _waypoints.Count == 0 || _isWaiting) 
                return _self.transform.position;
    
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
            _waitStartTime = Time.time;
        
            _waitForwardDirection = _self.transform.forward;
            if (_waitForwardDirection == Vector3.zero) _waitForwardDirection = Vector3.forward;
        
            float waitTime = _waypoints[_currentIndex].WaitTime;
        
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: _self.GetCancellationTokenOnDestroy());
        
            _currentIndex = (_currentIndex + 1) % _waypoints.Count;
            _isWaiting = false;
        }

        public IObservable<Vector2> OnRawMovement => Observable.Never<Vector2>();

        public float GetMovementSpeed(UnitStatsData stats) => stats.patrolSpeed;
        public IObservable<UniRx.Unit> OnShoot => Observable.Never<UniRx.Unit>();
        public IObservable<UniRx.Unit> OnAbilityUse => Observable.Never<UniRx.Unit>();
    }
}