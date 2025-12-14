using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Utils;
using _Project.Scripts.Utils.Extensions;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Unit.Mover
{
  public class MainCharacterMover : MonoBehaviour, IUnitMover
  {
    private ICameraService _cameraService;

    [Inject]
    public void Construct(ICameraService cameraService)
    {
      _cameraService = cameraService;
    }

    public void Move(GameUnit gameUnit, Vector2 movementDelta, float deltaTime)
    {
      var movement = CalculateMovement(gameUnit, movementDelta, deltaTime);
      UpdateAnimator(gameUnit, movementDelta, deltaTime, movement);
    }

    public void ResetMovement(GameUnit gameUnit)
    { 
      gameUnit.Animator.Idle();
    }

    private Vector3 CalculateMovement(GameUnit gameUnit, Vector2 movementDelta, float deltaTime)
    {
      var cameraForward = _cameraService.Camera.transform.forward.SetY(0f).normalized;
      var cameraRight = _cameraService.Camera.transform.right.SetY(0f).normalized;

      var movement = cameraForward * movementDelta.y + cameraRight * movementDelta.x;

      if (movement != Vector3.zero)
        gameUnit.transform.Translate(movement * deltaTime, Space.World);

      return movement;
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