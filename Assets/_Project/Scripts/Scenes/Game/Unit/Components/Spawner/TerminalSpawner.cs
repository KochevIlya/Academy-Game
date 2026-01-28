using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Scenes.Game.Unit.Components.Spawner
{
    public class TerminalSpawner : MonoBehaviour
    {
        [Header("Кого можно взломать через этот терминал")]
        public List<UnitSpawner> LinkedEnemies;

        [Header("Префаб самого терминала")]
        public AssetReferenceGameObject TerminalPrefabReference;
    
        public Vector3 Position => transform.position;
        public GameObject WarZonePoint;
        public Transform WarZoneTransform =>  WarZonePoint.transform; 
    }
}   
