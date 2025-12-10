using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.StaticData
{
  public interface IStaticDataService
  {
    void LoadAll();
    UnitsConfig UnitsConfig { get; }
  }
}