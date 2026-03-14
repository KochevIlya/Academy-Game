using _Project.Scripts.Infrastructure.Gui.Camera;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.Possesion;
using _Project.Scripts.Infrastructure.SaveLoad;
using _Project.Scripts.Infrastructure.UIMediator;
using _Project.Scripts.Scenes.Game.Hacking;
using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using _Project.Visual.UI.Menus.GameMenu;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Infrastructure
{
  public class GameServicesInstaller : MonoInstaller
  {
    [SerializeField] private CameraService _cameraService;
    [SerializeField] private GameObject _hackingPrefab;
    [SerializeField] private Transform _uiRoot;
    [SerializeField] private GameMenuWindow _menuPrefab;
    [SerializeField] private GuiService _guiServicePrefab;
    public override void InstallBindings()
    {
      
      Container.DeclareSignal<SaveRequestedSignal>();
      
      Container
        .BindInterfacesTo<GuiService>()
        .FromComponentInNewPrefab(_guiServicePrefab)
        .UnderTransform(_uiRoot)
        .AsSingle()
        .NonLazy();
      
      
      Container.Bind<IPlayerProvider>().To<PlayerProvider>().AsSingle();
      
      Container.Bind<ICameraService>().To<CameraService>().FromInstance(_cameraService).AsSingle();
      
      Container.Bind<IInputHelper>().To<InputHelper>().AsSingle();
      
      Container.Bind<IGameFactory>().To<GameFactory>().AsSingle();
      Container.Bind<InputControllsFactory>().AsSingle();
      
      Container.Bind<HackableSelector>().AsSingle();
      
      Container.BindInterfacesAndSelfTo<HackingService>().AsSingle();
      
      
      
      Container.Bind<HackingView>()
        .FromComponentInNewPrefab(_hackingPrefab) 
        .UnderTransform(_uiRoot)                      
        .AsSingle()                                   
        .NonLazy();
      
      Container.Bind<ICursorService>().To<CursorService>().AsSingle();
      Container.BindInterfacesAndSelfTo<UserInputControls>().AsSingle();
      Container.Bind<IPosessionService>().To<PosessionService>().AsSingle();
      
      
      Container.Bind<SceneLoaderService>().AsSingle();
      Container.Bind<ISaveLoadService>().To<SaveLoadService>().AsSingle();
      
      Container.BindSignal<SaveRequestedSignal>()
        .ToMethod<ISaveLoadService>(x => x.Save)
        .FromResolve();
      
      
      Container.Bind<IMenuActionsService>().To<MenuActionsService>().AsSingle();
      Container.BindInterfacesAndSelfTo<UIMediator>().AsSingle().NonLazy();
      
    }
  }
}