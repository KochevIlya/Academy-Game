using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    void Use(Vector3 targetPosition);
    bool CanUse();
}
