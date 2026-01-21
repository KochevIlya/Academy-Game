using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;

public class HackableComponent : MonoBehaviour
{
    
    [field: SerializeField] public int Difficulty { get; private set; } = 4;
    
    public GameUnit OwnerUnit { get; private set; }

    public void Initialize(GameUnit unit)
    {
        OwnerUnit = unit;
    }
}
