using UnityEngine;
using _Project.Scripts.Common.Extensions;
using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Unit;
using UniRx;


public class MainCharacterRotator : IUnitRotator
{
    private readonly ICameraService _cameraService;
    private readonly CompositeDisposable _disposable = new();
    private readonly Plane _plane = new Plane(Vector3.up, Vector3.zero);
    
    public MainCharacterRotator(IInputControls input, ICameraService cameraService, GameUnit unit)
    {
        _cameraService = cameraService;
        
        Observable.EveryUpdate()
            .Select(_ => input.MousePosition) 
            .DistinctUntilChanged()           
            .Subscribe(mousePos => Rotate(unit, mousePos))
            .AddTo(_disposable);
    }
        
    public void Rotate(GameUnit gameUnit, Vector2 movementDelta)
    {
        UpdateRotation(gameUnit, movementDelta);
    }
    
    private void UpdateRotation(GameUnit unit, Vector2 mouseScreenPos)
    {
        
        Ray ray = _cameraService.Camera.ScreenPointToRay(mouseScreenPos);

        if (_plane.Raycast(ray, out float enter))
        {
            Vector3 targetPoint = ray.GetPoint(enter);
            Vector3 direction = targetPoint - unit.transform.position;
            direction.y = 0; 
            if (direction != Vector3.zero)
            {
                unit.transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}
