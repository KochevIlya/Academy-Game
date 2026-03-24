using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using _Project.Scripts.Scenes.Game.Unit._Configs;
using _Project.Scripts.Scenes.Game.Unit._Data;
using UnityEngine;

[Serializable]
public class EnemySaveData
{
    public string Id;
    public UnitСharacteristicsType CharacteristicsType;
    public Vector3 Position;
    public int CurrentHealth;
    public bool IsMainCharacter;
    public List<WaypointData> customPath;
    // public string PatrolPathId;
}
