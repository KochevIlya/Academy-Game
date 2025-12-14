using _Project.Scripts.Scenes.Game.Shoot.Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.StaticData
{
  public interface IStaticDataService
  {
    void LoadAll();
    UnitsConfig UnitsConfig { get; }
    WeaponsConfig WeaponsConfig { get; }
  }
}