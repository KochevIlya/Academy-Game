using System.Collections.Generic;
using System.Threading;
using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Infrastructure.Gui.Screens;
using _Project.Scripts.Infrastructure.Gui.Service;
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
        private Transform _currentViewPoint;
        [Inject] private ICursorService _cursorService;
        [Inject] private IGuiGameService _guiGameService;
        
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
            _guiGameService.ShowHackingSelectionWindow();
            _cursorService.SetDefaultCursor();
            _cursorService.SetVisible(true);
            _cursorService.SetLockState(false);
            
            _cameraService.SetPoint(_currentViewPoint);

            HackableComponent lastHovered = null;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    Camera activeCamera = Camera.main;
                    if (activeCamera == null) 
                    {
                        await UniTask.Yield(PlayerLoopTiming.Update, token);
                        continue;
                    }
                    
                    HackableComponent currentHovered = ScanForTarget(activeCamera);

                    if (currentHovered != lastHovered)
                    {
                        if (lastHovered != null) SetOutlineState(lastHovered, false);
                        if (currentHovered != null) SetOutlineState(currentHovered, true);
                
                        lastHovered = currentHovered;
                    }

                    if (Input.GetMouseButtonDown(0) && currentHovered != null)
                    {
                        _cursorService.SetVisible(false);
                        _cursorService.SetLockState(true);
                        return currentHovered;
                    }

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                    
                }
                
            }
            finally
            {
                if (lastHovered != null) SetOutlineState(lastHovered, false);
                Debug.Log($"[HackableSelector] SelectTarget() GuiGameService.Pop()");
                await _guiGameService.CloseScreen(ScreenType.HackingSelectionWindow);
            }
            return null;
        }
        private HackableComponent ScanForTarget(Camera camera)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
    
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
    
            foreach (var hit in hits)
            {
                var hackable = hit.collider.GetComponentInParent<HackableComponent>();
                if (hackable != null) return hackable;
            }
            return null;
        }
        
        private void SetOutlineState(HackableComponent target, bool state)
        {
            if (target == null || target.Equals(null)) return;
    
            if (target.TryGetComponent<SimpleOutline>(out var outline))
            {
                outline.SetHighlight(state);
            }
        }
    }
}
