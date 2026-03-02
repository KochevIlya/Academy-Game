using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;

namespace _Project.Scripts.Tests.Editor
{
    public class FakeCameraService : ICameraService
    {
        private GameObject _cameraGameObject;
        public UnityEngine.Camera Camera { get; private set; }

        public FakeCameraService()
        {
            _cameraGameObject = new GameObject("FakeCamera");
            Camera = _cameraGameObject.AddComponent<UnityEngine.Camera>();
            Camera.transform.forward = Vector3.forward;
        }

        public void Init() { }

        public void SetTarget(GameUnit unit) { }

        public void SetPoint(Transform point){ }
        public void Cleanup() { }
        public void ZoomOut()
        {
            throw new System.NotImplementedException();
        }

        public void ResetZoom()
        {
            throw new System.NotImplementedException();
        }

        public void Destroy()
        {
            Object.DestroyImmediate(_cameraGameObject);
        }
    }
}