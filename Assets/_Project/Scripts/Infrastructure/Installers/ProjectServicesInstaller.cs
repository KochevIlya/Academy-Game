using _Project.Scripts.Infrastructure.AssetProvider;
using _Project.Scripts.Infrastructure.StaticData;
using _Project.Scripts.Libs.Configs.Loader;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using Zenject;

namespace _Project.Scripts.Infrastructure.Installers
{
  public class ProjectServicesInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.Bind<IStaticDataService>().To<StaticDataService>().AsSingle();
      Container.Bind<IConfigsLoader>().To<ConfigsLoader>().AsSingle();
      Container.Bind<IAssetProvider>().To<AssetProvider.AssetProvider>().AsSingle();
    }
  }
}