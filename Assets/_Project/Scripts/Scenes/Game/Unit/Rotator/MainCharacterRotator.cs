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
            Vector3 hit = ray.GetPoint(enter);
            Vector3 dir = hit - unit.transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                
                unit.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
        }
    }
}
