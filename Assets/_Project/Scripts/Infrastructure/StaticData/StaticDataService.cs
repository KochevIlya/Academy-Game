using _Project.Scripts.Libs.Configs.Loader;
using _Project.Scripts.Scenes.Game.Hacking.Terminal;
using _Project.Scripts.Scenes.Game.Shoot.Config;
using _Project.Scripts.Scenes.Game.Unit._Configs;

namespace _Project.Scripts.Infrastructure.StaticData
{
  public class StaticDataService : IStaticDataService
  {
    private readonly IConfigsLoader _configsLoader;

    public UnitsConfig UnitsConfig { get; private set; }
    public WeaponsConfig WeaponsConfig { get; private set; }
    public TerminalConfig TerminalConfig { get; private set; }
    
    public UnitStatsConfig UnitStatsConfig { get; private set; }
    

    public StaticDataService(IConfigsLoader configsLoader)
    {
      _configsLoader = configsLoader;
    }

    public void LoadAll()
    {
      UnitsConfig = _configsLoader.LoadSoConfig<UnitsConfig>();
      WeaponsConfig = _configsLoader.LoadSoConfig<WeaponsConfig>();
      TerminalConfig = _configsLoader.LoadSoConfig<TerminalConfig>();
      UnitStatsConfig = _configsLoader.LoadSoConfig<UnitStatsConfig>();
      
    }
  }
}