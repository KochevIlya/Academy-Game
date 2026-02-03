using System.Collections.Generic;
using _Project.Scripts.Libs.Configs.Variants;
using _Project.Scripts.Scenes.Game.Unit._Data;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit._Configs
{ 
    [CreateAssetMenu(menuName = "Configs/" + nameof(UnitStatsConfig), fileName = nameof(UnitStatsConfig))]
    public class UnitStatsConfig : SoConfig<UnitStatsConfig>
    {
        public Dictionary<UnitСharacteristicsType, UnitStatsData> Units;
    }
}