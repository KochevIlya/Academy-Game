using _Project.Scripts.Common.Extensions;
using _Project.Scripts.Infrastructure.Gui.Camera;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit
{
  public class MainCharacterMover : IUnitMover
  {
    private readonly ICameraService _cameraService;
        
    public MainCharacterMover(ICameraService cameraService)
    {
      _cameraService = cameraService;
    }
        
    public void Move(GameUnit gameUnit, Vector2 movementDelta)
    {
      var movement = UpdatePosition(gameUnit, movementDelta);
      UpdateAnimator(gameUnit, movement);
    }
    
    private Vector3 UpdatePosition(GameUnit gameUnit, Vector2 movementDelta)
    {
      Vector3 cameraForward = _cameraService.Camera.transform.forward.SetY(0f).normalized;
      Vector3 cameraRight = _cameraService.Camera.transform.right.SetY(0f).normalized;
            
      Vector3 movement = cameraForward * movementDelta.y + cameraRight * movementDelta.x;
            
      if (movement != Vector3.zero) 
        gameUnit.transform.Translate(movement * Time.deltaTime, Space.World);

      return movement;
    }

    private void UpdateAnimator(GameUnit gameUnit, Vector3 movement)
    {
      gameUnit.Animator.Run(movement.normalized.magnitude);
    }
  }
}