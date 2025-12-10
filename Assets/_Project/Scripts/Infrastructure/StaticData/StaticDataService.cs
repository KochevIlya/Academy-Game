using _Project.Scripts.Libs.Configs.Loader;

namespace _Project.Scripts.Infrastructure.StaticData
{
  public class StaticDataService : IStaticDataService
  {
    private readonly IConfigsLoader _configsLoader;

    public UnitsConfig UnitsConfig { get; private set; }

    public StaticDataService(IConfigsLoader configsLoader)
    {
      _configsLoader = configsLoader;
    }

    public void LoadAll()
    {
      UnitsConfig = _configsLoader.LoadSoConfig<UnitsConfig>();
    }
  }
}