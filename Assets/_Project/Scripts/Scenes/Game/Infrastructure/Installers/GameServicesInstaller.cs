using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Infrastructure
{
  public class GameServicesInstaller : MonoInstaller
  {
    [SerializeField] private CameraService _cameraService;
    [SerializeField] private GameObject _hackingPrefab;
    [SerializeField] private Transform _uiRoot;
    public override void InstallBindings()
    {
      
      
      Container.Bind<ICameraService>().To<CameraService>().FromInstance(_cameraService).AsSingle();
      Container.Bind<IInputHelper>().To<InputHelper>().AsSingle();
      Container.Bind<IGameFactory>().To<GameFactory>().AsSingle();

      Container.BindInterfacesAndSelfTo<DummyInputControls>().AsSingle();
      Container.BindInterfacesAndSelfTo<UserInputControls>().AsSingle();
      
      Container.BindInterfacesAndSelfTo<HackingService>().AsSingle();
      Container.Bind<HackingView>()
        .FromComponentInNewPrefab(_hackingPrefab) 
        .UnderTransform(_uiRoot)                      
        .AsSingle()                                   
        .NonLazy();
    }
  }
}