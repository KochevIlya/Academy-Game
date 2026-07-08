using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunSpawner : MonoBehaviour
{

    public ITurret SpawnedTurret { get; private set; }
    public Vector3 Position => transform.position;
    public void SetSpawnedTurret(ITurret turret)
    {
        SpawnedTurret = turret;   
    }

}
