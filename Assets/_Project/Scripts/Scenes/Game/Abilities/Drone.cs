using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;

public class Drone : GameUnit
{
    private float speed = 10f;
    private Vector3 _targetPosition;
    
    public void Setup(Vector3 targetPosition, float speed)
    {
        _targetPosition = targetPosition;
        this.speed = speed;
    }
}
