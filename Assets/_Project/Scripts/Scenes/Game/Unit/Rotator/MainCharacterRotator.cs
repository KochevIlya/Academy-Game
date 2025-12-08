using _Project.Scripts.Common.Extensions;
using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Utils;
using UnityEngine;
using Zenject;

public class MainCharacterRotator : MonoBehaviour, IUnitRotator
{
  [SerializeField] private float _smoothFactor = 15f;
  [SerializeField] private float _minAngleForAnimation = 1f;

  private ICameraService _cameraService;
  private readonly Plane _plane = new Plane(Vector3.up, Vector3.zero);

  [Inject]
  public void Construct(ICameraService cameraService)
  {
    _cameraService = cameraService;
  }

  public void Rotate(GameUnit gameUnit, Vector2 mouseScreenPos, float deltaTime)
  {
    var ray = _cameraService.Camera.ScreenPointToRay(mouseScreenPos);

    if (_plane.Raycast(ray, out var enter))
    {
      var targetDirection = (ray.GetPoint(enter) - gameUnit.transform.position).SetY(0f);

      if (targetDirection.sqrMagnitude < Constants.Epsilon) return;

      RotateUnit(gameUnit, deltaTime, targetDirection);
      AnimateRotation(gameUnit, deltaTime, targetDirection);
    }
  }

  private void RotateUnit(GameUnit gameUnit, float deltaTime, Vector3 targetDirection)
  {
    var targetRotation = Quaternion.LookRotation(targetDirection);
    gameUnit.transform.rotation = Quaternion.Slerp(
      gameUnit.transform.rotation,
      targetRotation,
      _smoothFactor * deltaTime
    );
  }

  private void AnimateRotation(GameUnit gameUnit, float deltaTime, Vector3 targetDirection)
  {
    var angle = Vector3.Angle(targetDirection, gameUnit.transform.forward);
    if (angle > _minAngleForAnimation)
    {
      var localDirection = gameUnit.transform.InverseTransformDirection(targetDirection);
      gameUnit.Animator.Rotate(new Vector2(localDirection.x, 0f).normalized, deltaTime);
    }
    else
      gameUnit.Animator.Rotate(Vector2.zero, deltaTime);
  }
}