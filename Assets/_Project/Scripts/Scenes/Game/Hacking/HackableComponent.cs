using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;

public class HackableComponent : MonoBehaviour, IHackable
{
    
    [field: SerializeField] public int Difficulty { get; private set; } = 4;
    protected bool _isActive;
    public GameUnit OwnerUnit { get; private set; }

    public void Initialize(GameUnit unit)
    {
        OwnerUnit = unit;
    }

    public virtual void Activate()
    {
        _isActive = true;
    }

    public virtual void Deactivate()
    {
        _isActive = false;
    }
    
}
