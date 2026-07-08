using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpawner : MonoBehaviour
{

    public TurretBase SpawnedTurret { get; private set; }
    public Vector3 Position => transform.position;
    public void SetSpawnedTurret(TurretBase turret)
    {
        SpawnedTurret = turret;   
    }

}
