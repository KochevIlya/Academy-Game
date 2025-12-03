using Sirenix.OdinInspector;

namespace _Project.Scripts.Libs.Configs.Variants
{
  public abstract class SoConfig<T> : SerializedScriptableObject, IConfig
  {
    public abstract T Data { get; }
  }
}