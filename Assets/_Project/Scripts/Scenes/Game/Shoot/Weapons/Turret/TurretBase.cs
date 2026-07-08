using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBase : MonoBehaviour, ITurret, IHackable
{

    protected float _hp;
    protected bool _isActive;
    
    
    
    public virtual float GetHP()
    {
        return _hp;
    }

    public virtual void SetHP(float hp)
    {
        _hp = hp;
    }

    public virtual bool IsActive()
    {
        return _isActive;
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
