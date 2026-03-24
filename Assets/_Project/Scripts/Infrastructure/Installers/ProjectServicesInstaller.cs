using _Project.Scripts.Infrastructure.AssetProvider;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.SaveLoad;
using _Project.Scripts.Infrastructure.StaticData;
using _Project.Scripts.Libs.Configs.Loader;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.Installers
{
  public class ProjectServicesInstaller : MonoInstaller
  {
    
    [SerializeField] private GuiService _guiServicePrefab;
    public override void InstallBindings()
    {
      Container
        .BindInterfacesTo<GuiService>()
        .FromComponentInNewPrefab(_guiServicePrefab)
        .AsSingle()
        .NonLazy();
      
      
      Container.Bind<IProgressService>().To<ProgressService>().AsSingle();
      Container.Bind<IStaticDataService>().To<StaticDataService>().AsSingle();
      Container.Bind<IConfigsLoader>().To<ConfigsLoader>().AsSingle();
      Container.Bind<IAssetProvider>().To<AssetProvider.AssetProvider>().AsSingle();
      Container.Bind<ICursorService>().To<CursorService>().AsSingle();
      Container.Bind<SceneLoaderService>().AsSingle();
      
      
      
      Container.Bind<IMenuActionsService>().To<MenuActionsService>().AsSingle();
      
      
    }
  }
}