using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IZoneSaveable : ISaveable
{
    CombatZoneSaveData GetSaveData();
    void LoadFromData(CombatZoneSaveData data);
}
