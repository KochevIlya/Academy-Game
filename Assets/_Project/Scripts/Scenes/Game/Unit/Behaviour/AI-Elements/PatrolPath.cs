using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    [SerializeField] bool needToShowPath = true;
    
    [Serializable]
    public struct PatrolPoint
    {
        public Transform PointTransform;
        public float WaitTime;
    }
    [SerializeField] private List<PatrolPoint> patrolPoints = new List<PatrolPoint>();
    private Dictionary<Transform, float> _waitTimesCache;
    private void Awake()
    {
        InitializeCache();
    }
    
    private void InitializeCache()
    {
        _waitTimesCache = new Dictionary<Transform, float>();
        foreach (var point in patrolPoints)
        {
            if (point.PointTransform != null)
                _waitTimesCache[point.PointTransform] = point.WaitTime;
        }
    }
    public List<PatrolPoint> GetPatrolPoints()
    {
        return patrolPoints;
    }
    public List<Vector3> GetPatrolPointsPositions()
    {
        List<Vector3> patrolPoints = new List<Vector3>();
        
        foreach (var child in transform.GetComponentsInChildren<Transform>())
        {
            patrolPoints.Add(child.position);
        }
      
        return  patrolPoints;
    }
    public float GetWaitTimeForPoint(Transform pointTransform)
    {
        if (_waitTimesCache == null) InitializeCache();
        
        return _waitTimesCache.TryGetValue(pointTransform, out var time) ? time : 0f;
    }
    private void OnDrawGizmos()
    {
        if (!needToShowPath || transform.childCount < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < patrolPoints.Count; i++)
        {
            if (patrolPoints[i].PointTransform == null) continue;
            
            int nextIndex = (i + 1) % patrolPoints.Count;
            if (patrolPoints[nextIndex].PointTransform == null) continue;
            
            Gizmos.DrawLine(patrolPoints[i].PointTransform.position, patrolPoints[nextIndex].PointTransform.position);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(patrolPoints[i].PointTransform.position, 0.3f);
            Gizmos.color = Color.green;
        }
    }
}
