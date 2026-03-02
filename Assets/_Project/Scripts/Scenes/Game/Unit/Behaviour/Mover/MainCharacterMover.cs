using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Utils;
using _Project.Scripts.Utils.Extensions;
using _Project.Scripts.Scenes.Game.Unit._Data;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Unit.Mover
{
  public class MainCharacterMover : MonoBehaviour, IUnitMover
  {
    //[SerializeField] private UnitStatsData _unitStatsData;
    
    private ICameraService _cameraService;
    private CharacterController _controller;
    
    private Vector3 _verticalVelocity;

    private const float Gravity = -9.81f;
    private const float GroundedGravity = -2f;
    
    [Inject]
    public void Construct(ICameraService cameraService)
    {
      _cameraService = cameraService;
    }

    private void Awake()
    {
      _controller = GetComponent<CharacterController>();
    }

    public void Move(GameUnit gameUnit, Vector3 movementDirection, float deltaTime, float speed)
    {
      movementDirection *= speed;
      Vector2 movementDelta = new Vector2(movementDirection.x, movementDirection.z);
      
      var movement = CalculateMovement(movementDelta); 
      ApplyGravity(deltaTime); 
      ApplyMovement(movement, deltaTime); 
      UpdateAnimator(gameUnit, movementDelta, deltaTime, movement);
    }

    public void ResetMovement(GameUnit gameUnit)
    { 
      gameUnit.Animator.Idle();
    }

    private Vector3 CalculateMovement(Vector2 movementDelta)
    {
      var cameraForward = _cameraService.Camera.transform.forward.SetY(0f).normalized; 
      var cameraRight = _cameraService.Camera.transform.right.SetY(0f).normalized;

      return (cameraForward * movementDelta.y + cameraRight * movementDelta.x); //* _unitStatsData.speed;
    }
    
    private void ApplyGravity(float deltaTime) 
    {
      if (_controller.isGrounded)
      {
        _verticalVelocity.y = GroundedGravity;
      }
      else
      {
        _verticalVelocity.y += Gravity * deltaTime;
      } 
    }

    private void ApplyMovement(Vector3 horizontal, float deltaTime)
    {
      var finalMovement = horizontal * deltaTime; 
      finalMovement += _verticalVelocity * deltaTime; 
      _controller.Move(finalMovement);
    }

    private void UpdateAnimator(GameUnit gameUnit, Vector2 movementDelta, float deltaTime, Vector3 movement)
    {
      if (movementDelta.sqrMagnitude > Constants.Epsilon)
      {
        var localDirection = gameUnit.transform.InverseTransformDirection(movement);
        gameUnit.Animator.Run(new Vector2(localDirection.x, localDirection.z).normalized, deltaTime);
      }
      else
      {
        gameUnit.Animator.Run(Vector2.zero, deltaTime);
      }
    }
  }
}