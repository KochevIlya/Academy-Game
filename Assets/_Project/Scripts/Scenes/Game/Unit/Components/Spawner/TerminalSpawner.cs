using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Scenes.Game.Unit.Components.Spawner
{
    public class TerminalSpawner : MonoBehaviour
    {
        private string _cachedId;

        public string SpawnerId 
        {
            get 
            {
                if (string.IsNullOrEmpty(_cachedId))
                {
                    var identifier = GetComponent<EntityIdentifier>();
                    if (identifier != null)
                        _cachedId = identifier.ID;
                    else
                        Debug.LogError($"[TerminalSpawner] На объекте {gameObject.name} нет EntityIdentifier!");
                }
                return _cachedId;
            }
        }
        public GameObject SpawnedTerminalObject { get; set; }
        public Vector3 Position => transform.position;
        public GameObject WarZonePoint;
        public Transform WarZoneTransform =>  WarZonePoint.transform; 
    }
}   
