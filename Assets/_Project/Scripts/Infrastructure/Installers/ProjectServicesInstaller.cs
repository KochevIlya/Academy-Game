using _Project.Scripts.Infrastructure.StaticData;
using _Project.Scripts.Libs.Configs.Loader;
using Zenject;

namespace _Project.Scripts.Infrastructure.Installers
{
  public class ProjectServicesInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.Bind<IStaticDataService>().To<StaticDataService>().AsSingle();
      Container.Bind<IConfigsLoader>().To<ConfigsLoader>().AsSingle();
    }
  }
}