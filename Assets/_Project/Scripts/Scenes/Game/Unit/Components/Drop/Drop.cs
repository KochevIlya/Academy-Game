using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit.Components.Drop
{
    public class Drop : MonoBehaviour
    {
        [SerializeField] GameObject _droppedObject;

        public void DropObject(Vector3 position)
        {
            Instantiate(_droppedObject, position, Quaternion.identity);
        }
    }
}