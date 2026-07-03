using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    private float speed = 10f;
    private Vector3 _targetPosition;
    
    public void Setup(Vector3 targetPosition, float speed)
    {
        _targetPosition = targetPosition;
        this.speed = speed;
    }
}
