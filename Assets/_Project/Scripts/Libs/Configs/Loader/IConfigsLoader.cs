using UnityEngine;

namespace _Project.Scripts.Libs.Configs.Loader
{
  public interface IConfigsLoader
  {
    T LoadSoConfig<T>() where T : Object, IConfig;
  }
}