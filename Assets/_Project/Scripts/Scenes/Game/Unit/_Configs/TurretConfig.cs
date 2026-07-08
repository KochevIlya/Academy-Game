using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Libs.Configs.Variants;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Configs/" + nameof(TurretConfig), fileName = nameof(TurretConfig))]
public class TurretConfig : SoConfig<TurretConfig>
{
    public AssetReference Prefab;
}
