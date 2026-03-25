using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Configs;
using _Project.Visual.Abilities;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Abilities
{
    public class ShieldAbility : BaseAbility<ShieldSettings>
    {
        private float _duration = 5f;
        private float _speedBuf = 0.1f;
        private float _damageBuf = 0.1f;
        private GameUnit _unit;
        private ShieldVisual _shieldVisual;
        public override void Initialize(GameUnit unit, AbilityConfig config)
        {
            base.Initialize(unit, config);
    
            _unit = unit;

            _shieldVisual = unit.GetComponentInChildren<ShieldVisual>();
            var shieldSettings = config.GetSettings(BotAbilityType.Shield) as ShieldSettings;
    
            if (shieldSettings != null)
            {
                Debug.Log("$shield Ability settings loaded!");
                _settings = shieldSettings; 
                _duration = shieldSettings.duration;
                _speedBuf = 1f - shieldSettings.speedBufPercent * 0.01f;
                _damageBuf = 1f - shieldSettings.damageBufPercent * 0.01f;
                Debug.Log($"DamageBuf = {_damageBuf}");
        
                _timer = 0f;
                _isReady.Value = true;
            }
            else
            {
                Debug.LogError($"Shield settings not found in {config.name}!");
                enabled = false;
            }
        }

        public override void Use(Vector3 targetPosition)
        {
            if (!CanUse()) return;
            _isReady.Value = false;   
            UseAbility().Forget();

            if (_shieldVisual)
            {
                _shieldVisual.ChangeShieldVisual(true);
            }
            
            Observable.Timer(System.TimeSpan.FromSeconds(_duration))
                .Subscribe(_ => Deactivate(_unit))
                .AddTo(_unit);
            
        }
        private void Deactivate(GameUnit unit)
        {
            unit.SpeedMultiplier = 1f;
            unit.Health.IncomingDamageMultiplier = 1f;
            
            if (_shieldVisual)
            {
                _shieldVisual.ChangeShieldVisual(false);
            }
            _timer = _settings.cooldown;
        }
        protected override async UniTask UseAbility()
        {
            _unit.SpeedMultiplier = _speedBuf;
            _unit.Health.IncomingDamageMultiplier = _damageBuf;
            
        }

        public override BotAbilityType GetAbilityType()
        {
            return BotAbilityType.Shield;
        }
        
    }
}
