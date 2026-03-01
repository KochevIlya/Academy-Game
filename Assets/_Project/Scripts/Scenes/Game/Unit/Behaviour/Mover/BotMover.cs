using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Mover;
using UnityEngine;

public class BotMover : MonoBehaviour, IUnitMover
{
    private const float _gravity = -9.81f; 
    private const float _groundGravity = -2f;
    private Vector3 _verticalVelocity;
    private CharacterController _controller;
    
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }
    public void Move(GameUnit gameUnit, Vector2 movementDelta, float deltaTime)
    {
        Vector3 horizontalDirection = CalculateMovementDirection(movementDelta);
    
        ApplyGravity(deltaTime);
    
        Vector3 finalMovement = CalculateFinalMovement(horizontalDirection, deltaTime);
    
        _controller.Move(finalMovement);
    
        UpdateAnimator(gameUnit, horizontalDirection, deltaTime);
    }
    
    private Vector3 CalculateMovementDirection(Vector2 movementDelta)
    {
        Vector3 direction = (transform.forward * movementDelta.y + transform.right * movementDelta.x);
        
        
        return direction;
    }

    private void ApplyGravity(float deltaTime)
    {
        if (_controller.isGrounded)
        {
            _verticalVelocity.y = _groundGravity;
        }
        else
        {
            _verticalVelocity.y += _gravity * deltaTime;
        }
    }

    private Vector3 CalculateFinalMovement(Vector3 horizontalDirection, float deltaTime)
    {
        return (horizontalDirection + _verticalVelocity) * deltaTime;
    }

    private void UpdateAnimator(GameUnit gameUnit, Vector3 movementDirection, float deltaTime)
    {
        Vector3 localDirection = transform.InverseTransformDirection(movementDirection);
        
        gameUnit.Animator.Run(new Vector2(localDirection.x, localDirection.z), deltaTime);
    }

    public void ResetMovement(GameUnit gameUnit)
    {
        gameUnit.Animator.Idle();
    }
}
