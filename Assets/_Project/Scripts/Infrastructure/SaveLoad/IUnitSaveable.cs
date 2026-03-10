using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitSaveable : ISaveable
{

    EnemySaveData GetSaveData();
    void LoadFromData(EnemySaveData data);
    void DestroyEntity();
}
