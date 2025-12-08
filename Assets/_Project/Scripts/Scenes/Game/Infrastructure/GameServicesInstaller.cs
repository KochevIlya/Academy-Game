using _Project.Scripts.Infrastructure.Gui.Camera;
using UnityEngine;
using Zenject;

public class GameServicesInstaller : MonoInstaller
{
  [SerializeField] private CameraService _cameraService;

  public override void InstallBindings()
  {
    Container.Bind<ICameraService>().To<CameraService>().FromInstance(_cameraService).AsSingle();
  }
}