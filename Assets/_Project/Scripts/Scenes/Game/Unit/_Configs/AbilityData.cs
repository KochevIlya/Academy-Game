using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit._Configs;
using _Project.Scripts.Scenes.Game.Unit._Data;
using UnityEngine;

[Serializable]
public class AbilityData
{
    public BotAbilityType abilityType;
    
    [SerializeReference]
    public AbilitySettings settings;
}