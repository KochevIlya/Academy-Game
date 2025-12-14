using _Project.Scripts.Libs.Configs.Variants;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Configs/" + nameof(UnitsConfig), fileName = nameof(UnitsConfig))]
public class UnitsConfig : SoConfig<UnitsConfig>
{
  public AssetReference Character;
  public AssetReference Bot;
}