using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Components.Drop
{
    public class DroppedObject : MonoBehaviour
    {
        [SerializeField] private int addHealth = 0;
        
        public void Consume(GameUnit unit)
        {
            if (addHealth != 0) unit.Health.AddHealth(addHealth);
            
            Destroy(gameObject);
        }
    }
}
