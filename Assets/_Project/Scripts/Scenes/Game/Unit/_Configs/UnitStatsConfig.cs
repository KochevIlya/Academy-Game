using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Unit._Configs
{
    [CreateAssetMenu(menuName = "Configs/" + nameof(UnitStatsConfig), fileName = nameof(UnitStatsConfig))]
    public class UnitStatsConfig : ScriptableObject
    {
        [Header("Main Stats")]
        public int maxHealth = 100;
        public float speed = 3f;
    }
}
