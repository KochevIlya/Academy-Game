using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    [SerializeField] bool needToShowPath = true;
    
    public List<Vector3> GetPatrolPointsPositions()
    {
        List<Vector3> patrolPoints = new List<Vector3>();
        
        foreach (var child in transform.GetComponentsInChildren<Transform>())
        {
            patrolPoints.Add(child.position);
        }
      
        return  patrolPoints;
    }
    
    private void OnDrawGizmos()
    {
        if (!needToShowPath || transform.childCount < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }

        Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position, transform.GetChild(0).position);
    }
}
