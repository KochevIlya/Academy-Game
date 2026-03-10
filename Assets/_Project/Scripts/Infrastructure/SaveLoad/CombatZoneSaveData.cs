using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CombatZoneSaveData
{
    public string ZoneId; 
    public List<string> ActiveUnitIds = new List<string>();
}
