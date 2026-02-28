using System;
using _Project.Scripts.Scenes.Game.Shoot.Data;

namespace _Project.Scripts.Scenes.Game.Unit._Data
{
    
    [Serializable]
    public enum UnitBehaviourType
    {
        Character,
        Melee,  
        Distance 
    }
    public class UnitStatsData
    {
        public int maxHealth = 100;
        public float speed = 3f;
        public UnitBehaviourType behaviourType;
        public WeaponType weaponType;
        
    }
}