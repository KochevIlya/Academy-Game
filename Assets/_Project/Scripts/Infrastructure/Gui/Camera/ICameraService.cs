using UnityEngine;

namespace _Project.Scripts.Infrastructure.Gui.Camera
{
    public interface ICameraService
    {
        UnityEngine.Camera Camera { get; }
        void Init();
        void SetTarget(Transform target);
        void Cleanup();
    }
}