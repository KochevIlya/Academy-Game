using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Libs.Configs.Variants;
using UnityEngine;

using UnityEngine;
using _Project.Scripts.Scenes.Game.Unit._Data;

namespace _Project.Scripts.Scenes.Game.Unit._Configs
{
    
    public enum BotAbilityType
    {
        None,
        ThrowGrenade
    }

    [Serializable]
    public abstract class AbilitySettings
    {
    }
    [Serializable]
    public class GrenadeSettings : AbilitySettings
    {
        public float cooldown = 3f;
        public int damage = 20;
        public float radius = 3f;
        public float fuseTime = 2f;
        public float speed = 5f;
    }
    
    
    [CreateAssetMenu(menuName = "Configs/" + nameof(AbilityConfig), fileName = "NewAbilityConfig")]
    public class AbilityConfig : SoConfig<AbilityConfig>
    {
        public List<AbilityData> Abilities;
        public AbilitySettings GetSettings(BotAbilityType type)
        {
            var data = Abilities.FirstOrDefault(a => a.abilityType == type);
            return data?.settings;
        }
    }
}
