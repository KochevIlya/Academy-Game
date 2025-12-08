using _Project.Scripts.Common.Extensions;
using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;
using Zenject;

public class MainCharacterRotator : MonoBehaviour, IUnitRotator
{
    [SerializeField] private float _rotationSharpness = 15f;
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
        Ray ray = _cameraService.Camera.ScreenPointToRay(mouseScreenPos);

        if (_plane.Raycast(ray, out float enter))
        {
            Vector3 targetPoint = ray.GetPoint(enter);
            Vector3 targetDirection = (targetPoint - gameUnit.transform.position).SetY(0f);
            
            if (targetDirection.sqrMagnitude < 0.01f)
            {
                gameUnit.Animator.Rotate(Vector2.zero, deltaTime);
                return;
            }
            
            float angle = Vector3.Angle(targetDirection, gameUnit.transform.forward);
            
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            gameUnit.transform.rotation = Quaternion.Slerp(
                gameUnit.transform.rotation,
                targetRotation,
                _rotationSharpness * deltaTime
            );
            
            Vector2 targetAnimParams;
            if (angle > _minAngleForAnimation)
            {
                var localDirection = gameUnit.transform.InverseTransformDirection(targetDirection);
                targetAnimParams = new Vector2(localDirection.x, 0f).normalized;
            }
            else
            {
                targetAnimParams = Vector2.zero;
            }
            
            gameUnit.Animator.Rotate(targetAnimParams, deltaTime);
        }
    }
}