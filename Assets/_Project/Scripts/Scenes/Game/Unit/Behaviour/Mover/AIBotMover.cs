using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Mover;
using UnityEngine;
using UnityEngine.AI;

public class AIBotMover : MonoBehaviour, IUnitMover
{

    private NavMeshAgent _agent;
    
    private const float _gravity = -9.81f; 
    private const float _groundGravity = -2f;
    private Vector3 _verticalVelocity;

    private void Awake()
    {
        if (_agent == null) _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = true;
    }

    public void Move(GameUnit gameUnit, Vector3 targetPosition, float deltaTime, float speed)
    {
        if (!_agent.isOnNavMesh) return;
        
        _agent.speed = speed;
        _agent.SetDestination(targetPosition);

        UpdateAnimator(gameUnit);
    }

    public void ResetMovement(GameUnit gameUnit)
    {
        if (_agent.isActiveAndEnabled && _agent.isOnNavMesh)
        {
            _agent.ResetPath();
        }
        gameUnit.Animator.Idle();
    }

    private void UpdateAnimator(GameUnit gameUnit)
    {
        Vector3 localVelocity = transform.InverseTransformDirection(_agent.velocity);
        
        Vector2 animDir = new Vector2(localVelocity.x, localVelocity.z) / _agent.speed;
        
        gameUnit.Animator.Run(animDir, Time.deltaTime);
    }
}
