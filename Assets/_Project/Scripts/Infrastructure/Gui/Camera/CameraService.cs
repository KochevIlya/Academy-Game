using Unity.Cinemachine;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Gui.Camera
{
    public sealed class CameraService : MonoBehaviour, ICameraService
    {
        [SerializeField] private UnityEngine.Camera _camera;
        [SerializeField] private CinemachineCamera _cameraZoomIn;
        
        UnityEngine.Camera ICameraService.Camera => _camera;

        void ICameraService.Init()
        {
            
        }

        void ICameraService.SetTarget(Transform target)
        {
            _cameraZoomIn.Follow = target;
        }

        void ICameraService.Cleanup()
        {
            _cameraZoomIn.Follow = null;
        }
    }
}