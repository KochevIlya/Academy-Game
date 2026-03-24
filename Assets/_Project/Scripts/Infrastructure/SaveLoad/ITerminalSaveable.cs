using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITerminalSaveable : ISaveable
{
    TriggerSaveData GetSaveData();
    void LoadFromData(TriggerSaveData data);
}
