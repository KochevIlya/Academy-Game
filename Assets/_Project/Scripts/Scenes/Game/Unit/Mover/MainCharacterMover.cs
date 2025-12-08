using _Project.Scripts.Common.Extensions;
using _Project.Scripts.Infrastructure.Gui.Camera;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Unit
{
  public class MainCharacterMover : MonoBehaviour, IUnitMover
  {
    [SerializeField] private float _interpolateValue = 0.1f;
    [SerializeField] private float _movementThreshold = 0.01f;

    private ICameraService _cameraService;

    [Inject]
    public void Construct(ICameraService cameraService)
    {
      _cameraService = cameraService;
    }

    public void Move(GameUnit gameUnit, Vector2 movementDelta, float deltaTime)
    {
      var movement = UpdatePosition(gameUnit, movementDelta, deltaTime);
      
      if (movementDelta.sqrMagnitude > _movementThreshold * _movementThreshold)
        UpdateAnimator(gameUnit, gameUnit.transform.InverseTransformDirection(movement));
      else
        gameUnit.Animator.Run(Vector2.zero, _interpolateValue);
    }

    private Vector3 UpdatePosition(GameUnit gameUnit, Vector2 movementDelta, float deltaTime)
    {
      var cameraForward = _cameraService.Camera.transform.forward.SetY(0f).normalized;
      var cameraRight = _cameraService.Camera.transform.right.SetY(0f).normalized;

      var movement = cameraForward * movementDelta.y + cameraRight * movementDelta.x;

      if (movement != Vector3.zero)
        gameUnit.transform.Translate(movement * deltaTime, Space.World);

      return movement;
    }

    private void UpdateAnimator(GameUnit gameUnit, Vector3 movement)
    {
      gameUnit.Animator.Run(new Vector2(movement.x, movement.z).normalized, _interpolateValue);
    }
  }
}