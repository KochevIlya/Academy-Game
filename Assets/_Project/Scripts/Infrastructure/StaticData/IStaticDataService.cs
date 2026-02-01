using _Project.Scripts.Scenes.Game.Hacking.Terminal;
using _Project.Scripts.Scenes.Game.Shoot.Config;
using _Project.Scripts.Scenes.Game.Unit._Configs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.StaticData
{
  public interface IStaticDataService
  {
    void LoadAll();
    UnitsConfig UnitsConfig { get; }
    WeaponsConfig WeaponsConfig { get; }
    TerminalConfig TerminalConfig { get; }
    UnitStatsConfig UnitStatsConfig { get; }
  }
}