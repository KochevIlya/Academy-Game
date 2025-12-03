using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Libs.Configs.Loader
{
  public class ConfigsLoader : IConfigsLoader
  {
    private const string SoFolder = "Configs/So/";

    public T LoadSoConfig<T>() where T : Object, IConfig
    {
      var config = Resources.Load<T>(SoFolder + typeof(T).Name);

      if (config == null)
        throw new Exception($"Can't load config at '{SoFolder}{typeof(T).Name}'");

      return config;
    }

    private string ConfigName<T>() where T : IConfig => typeof(T).Name;
  }
}