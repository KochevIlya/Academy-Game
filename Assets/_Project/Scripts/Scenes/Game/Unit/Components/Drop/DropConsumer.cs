using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Components.Drop
{
    public class DropConsumer : MonoBehaviour
    {
        [SerializeField] GameUnit gameUnit;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<DroppedObject>(out var droppedObject))
            {
                Debug.Log("===================== CONSUMER ================");
                Debug.Log("FOUND!");
                if (!gameUnit) return;
                Debug.Log("CONSUME!");
                droppedObject.Consume(gameUnit);
            }
        }
    }
}
