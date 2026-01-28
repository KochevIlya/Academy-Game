using System.Collections.Generic;
using _Project.Scripts.Libs.Configs.Variants;
using _Project.Scripts.Scenes.Game.Shoot.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Scenes.Game.Hacking.Terminal
{
    [CreateAssetMenu(menuName = "Configs/" + nameof(TerminalConfig), fileName = nameof(TerminalConfig))]
    public class TerminalConfig : SoConfig<TerminalConfig>
    {
        public AssetReference Prefab;
    }
}
