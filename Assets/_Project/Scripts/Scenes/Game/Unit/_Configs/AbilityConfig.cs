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
        ThrowGrenade,
        Dash
    }

    [Serializable]
    public abstract class AbilitySettings
    {
        public float cooldown = 3f;
    }
    [Serializable]
    public class GrenadeSettings : AbilitySettings
    {
        public int damage = 20;
        public float radius = 3f;
        public float fuseTime = 2f;
        public float speed = 5f;
    }

    [Serializable]
    public class DashSettings : AbilitySettings
    {
        public float distance = 3f;
        public float speed = 20f;
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
