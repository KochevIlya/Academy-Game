using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Infrastructure
{
  public class GameServicesInstaller : MonoInstaller
  {
    [SerializeField] private CameraService _cameraService;

    public override void InstallBindings()
    {
      Container.Bind<ICameraService>().To<CameraService>().FromInstance(_cameraService).AsSingle();

      Container.Bind<IGameFactory>().To<GameFactory>().AsSingle();

      Container.BindInterfacesAndSelfTo<DummyInputControls>().AsSingle();
      Container.BindInterfacesAndSelfTo<UserInputControls>().AsSingle();
    }
  }
}