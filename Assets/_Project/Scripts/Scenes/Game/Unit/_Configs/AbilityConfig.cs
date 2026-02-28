using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Libs.Configs.Variants;
using UnityEngine;

using UnityEngine;
using _Project.Scripts.Scenes.Game.Unit._Data;

namespace _Project.Scripts.Scenes.Game.Unit._Configs
{
    [Serializable]
    public abstract class AbilitySettings
    {
    }
    [Serializable]
    public class GrenadeSettings : AbilitySettings
    {
        public float cooldown = 3f;
        public float damage = 20f;
        public float radius = 3f;
        public float fuseTime = 2f;
    }
    [CreateAssetMenu(menuName = "Configs/" + nameof(AbilityConfig), fileName = "NewAbilityConfig")]
    public class AbilityConfig : SoConfig<AbilityConfig>
    {
        [SerializeReference] 
        public AbilitySettings settings;
    }
}
