using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public List<EnemySaveData> enemies = new List<EnemySaveData>();
    public List<CombatZoneSaveData> zones = new List<CombatZoneSaveData>();
    public List<TriggerSaveData> triggers = new List<TriggerSaveData>();
}
