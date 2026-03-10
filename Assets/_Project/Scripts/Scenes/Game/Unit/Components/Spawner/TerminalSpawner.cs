using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Scenes.Game.Unit.Components.Spawner
{
    public class TerminalSpawner : MonoBehaviour
    {
        public GameObject SpawnedTerminalObject { get; set; }
        public Vector3 Position => transform.position;
        public GameObject WarZonePoint;
        public Transform WarZoneTransform =>  WarZonePoint.transform; 
    }
}   
