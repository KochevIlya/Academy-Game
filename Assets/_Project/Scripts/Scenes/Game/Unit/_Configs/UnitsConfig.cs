using System;
using System.Collections.Generic;
using _Project.Scripts.Libs.Configs.Variants;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Data;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Configs/" + nameof(UnitsConfig), fileName = nameof(UnitsConfig))]
public class UnitsConfig : SoConfig<UnitsConfig>
{
  public AssetReference Crosshair;
  
  [Serializable]
  public struct BehaviourPrefabMap
  {
    public UnitBehaviourType Behaviour;
    public AssetReference Prefab;
  }
  public List<BehaviourPrefabMap> BehaviourMaps;
  private Dictionary<UnitBehaviourType, AssetReference> _map;
  
  public AssetReference GetPrefabForBehaviour(UnitBehaviourType behaviourType)
  {
    if (_map == null) InitializeMap();
        
    if (_map.TryGetValue(behaviourType, out var prefab))
      return prefab;
            
    Debug.LogError($"Prefab not found for behaviour: {behaviourType}");
    return null;
  }

  private void InitializeMap()
  {
    _map = new Dictionary<UnitBehaviourType, AssetReference>();
    foreach (var map in BehaviourMaps)
    {
      if (!_map.ContainsKey(map.Behaviour))
        _map.Add(map.Behaviour, map.Prefab);
    }
  }
  
}