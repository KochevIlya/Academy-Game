using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurret
{
    public float GetHP();
    public void SetHP(float HP);

    public bool IsActive();

    public void Activate();
    public void Deactivate();
    
    

}
