using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit.Components.Spawner;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TerminalSpawner : MonoBehaviour
{
    [Header("Кого можно взломать через этот терминал")]
    public List<UnitSpawner> LinkedEnemies;

    [Header("Префаб самого терминала")]
    public AssetReferenceGameObject TerminalPrefabReference;
    
    public Vector3 Position => transform.position;
}
