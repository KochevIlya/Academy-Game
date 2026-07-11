using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;
using Zenject;

public class Drone : GameUnit
{
    private float speed = 10f;
    private Vector3 _targetPosition;
    private GameUnit _owner;
    private IPosessionService _posessionService;

    [Inject]
    public void Construct(GameUnit owner)
    {
        _owner = owner;
    }
    
    public void Setup(Vector3 targetPosition, float speed)
    {
        _targetPosition = targetPosition;
        this.speed = speed;
    }
    
    private void Update()
    {
        if (IsUnderControl && Input.GetKeyDown(KeyCode.Q))
        {
            ReturnControlToOwner();
        }
    }

    private void ReturnControlToOwner()
    {
        if (_owner != null && _posessionService != null)
        {
            Debug.Log("Возврат управления на владельца: " + _owner.name);
            
            _posessionService.Possess(_owner); 
            
            Destroy(gameObject); 
        }
    }
}
