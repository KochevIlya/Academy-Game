using System.Collections.Generic;
using System.Threading;
using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Unit;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Hacking
{
    public class HackableSelector
    {
        [Inject] private ICameraService _cameraService;
        private readonly Camera _mainCamera;
        private Transform _currentViewPoint;
        
        public HackableSelector()
        {
            _mainCamera = Camera.main;
        }
        public void ClearContext()
        {
            _currentViewPoint = null;
        }
        public void SetContext(Transform viewPoint)
        {
            Debug.Log("In Setting Context");
            _currentViewPoint = viewPoint;
        }
        
        public async UniTask<HackableComponent> SelectTarget(CancellationToken token)
        {
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Debug.Log($"[Selector] Камера летит в: {_currentViewPoint.name}");
            _cameraService.SetPoint(_currentViewPoint);
            try
            {
                while (true)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        var target = ScanForTarget();
                        if (target != null) return target;
                    }
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            }
            finally
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private HackableComponent ScanForTarget()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
    
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                var hackable = hit.collider.GetComponentInParent<HackableComponent>();

                if (hackable != null)
                {
                    Debug.Log($"[Selector] Цель опознана: {hackable.name}");
                    return hackable;
                }
            }
            return null;
        }
    }
}
