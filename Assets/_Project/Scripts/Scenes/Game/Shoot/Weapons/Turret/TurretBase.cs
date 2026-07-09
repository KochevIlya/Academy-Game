using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBase : HackableComponent, ITurret
{

    protected float _hp;
    
    
    
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
    
    
    
}
