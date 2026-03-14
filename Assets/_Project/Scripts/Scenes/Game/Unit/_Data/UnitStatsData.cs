using System;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using _Project.Scripts.Scenes.Game.Unit._Configs;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit._Data
{
    
    
    public enum UnitBehaviourType
    {
        Character,
        Melee,  
        Distance, 
        Tank
    }
    [Serializable]
    public class UnitStatsData
    {
        public int maxHealth = 100;
        public float speed = 3f;
        public float patrolSpeed = 1f;
        public UnitBehaviourType behaviourType;
        public WeaponType weaponType;
        
        public BotAbilityType abilityType;
        public AbilityConfig ability;
    }
}