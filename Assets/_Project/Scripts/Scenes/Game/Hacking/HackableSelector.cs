using System.Collections.Generic;
using System.Threading;
using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Unit;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
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
            _currentViewPoint = viewPoint;
        }
        
        public async UniTask<HackableComponent> SelectTarget(CancellationToken token)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _cameraService.SetPoint(_currentViewPoint);

            HackableComponent lastHovered = null;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    HackableComponent currentHovered = ScanForTarget();

                    if (currentHovered != lastHovered)
                    {
                        if (lastHovered != null) SetOutlineState(lastHovered, false);
                        if (currentHovered != null) SetOutlineState(currentHovered, true);
                
                        lastHovered = currentHovered;
                    }

                    if (Input.GetMouseButtonDown(0) && currentHovered != null)
                    {
                        return currentHovered;
                    }

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            }
            finally
            {
                if (lastHovered != null) SetOutlineState(lastHovered, false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            return null;
        }
        private HackableComponent ScanForTarget()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                var hackable = hit.collider.GetComponentInParent<HackableComponent>();

                if (hackable != null)
                {
                    return hackable;
                }

            }
            return null;

        }
        private void SetOutlineState(HackableComponent target, bool state)
        {
            if (target == null) return;
    
            if (target.TryGetComponent<SimpleOutline>(out var outline))
            {
                outline.SetHighlight(state);
            }
        }
    }
}
