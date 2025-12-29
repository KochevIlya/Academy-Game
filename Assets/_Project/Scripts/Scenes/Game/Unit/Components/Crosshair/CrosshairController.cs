using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using UniRx;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] private Transform _crosshairView;
    
    private Camera _camera;
    private readonly Plane _floorPlane = new Plane(Vector3.up, Vector3.zero);
    private CompositeDisposable _disposable = new CompositeDisposable();

    private void Start()
    {
        
        Cursor.visible = false; 
    }

    public void Initialize(IInputControls inputControls)
    {
        _camera = Camera.main;
        _disposable.Clear();
        Observable.EveryUpdate()
            .Select(_ => inputControls.MousePosition)
            .Subscribe(MoveCrosshairOnFloor)
            .AddTo(_disposable);
    }

    private void MoveCrosshairOnFloor(Vector2 screenMousePos)
    {
        Ray ray = _camera.ScreenPointToRay(screenMousePos);
        if (_floorPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            _crosshairView.position = hitPoint + Vector3.up;
            _crosshairView.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }

    private void OnDestroy()
    {
        _disposable.Clear();
        Cursor.visible = true;
    }
    
}
