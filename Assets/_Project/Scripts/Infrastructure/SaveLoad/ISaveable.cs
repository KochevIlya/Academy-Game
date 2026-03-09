using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    EnemySaveData GetSaveData();
    void LoadFromData(EnemySaveData data);
    void DestroyEntity();
}
