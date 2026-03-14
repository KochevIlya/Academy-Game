using System;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Configs;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Abilities
{
    public class DashAbility : BaseAbility<DashSettings>
    {
        
        private CharacterController _characterController;
        
        protected override async UniTask UseAbility()
        {
            Debug.Log($"Grenade In Use Grenade");
            
            if (_settings == null || _characterController == null)
            {
                _input.IsBlocked.Value = false;
                return;
            }
            _isReady.Value = false;
            for (var i = 0; i < Settings.jumpNumber; i++)
            {
                if (_settings == null || _characterController == null)
                {
                    _input.IsBlocked.Value = false;
                    return;
                }
                Vector2 mouseScreenPos = _input.MousePosition;
                if (!_inputHelper.ScreenToGroundPosition(mouseScreenPos, _unit.transform.position.y,
                        out Vector3 targetWorldPos))
                {
                    _input.IsBlocked.Value = false;
                    return;
                }

                Vector3 direction = targetWorldPos - _unit.transform.position;
                direction.y = 0f;
                direction.Normalize();

                _input.IsBlocked.Value = true;

                float distanceDashed = 0f;

                while (distanceDashed < Settings.distance)
                {
                    float step = Settings.speed * Time.deltaTime;
                    if (_settings == null || _characterController == null)
                    {
                        _input.IsBlocked.Value = false;
                        return;
                    }
                    CollisionFlags flags = _characterController.Move(direction * step);

                    distanceDashed += step;

                    if ((flags & CollisionFlags.Sides) != 0)
                    {
                        break;
                    }

                    await UniTask.Yield(PlayerLoopTiming.Update);
                }   
                
                await UniTask.Delay(TimeSpan.FromSeconds(Settings.timeout));
            }

            _input.IsBlocked.Value = false;
            _timer = _settings.cooldown;
        }

        public void Initialize(GameUnit unit, AbilityConfig config)
        {
            base.Initialize(unit, config);
            
            _characterController = unit.GetComponent<CharacterController>();
            var settings = _abilityConfig.GetSettings(BotAbilityType.Dash) as DashSettings;
            
            if (settings != null)
            {
                _settings = settings;
                _timer = 0f;
                _isReady.Value = true;
            }
            else
            {
                Debug.LogError($"Dash settings not found in {_abilityConfig.name}");
                enabled = false;
            }
            
        }

        public override BotAbilityType GetAbilityType()
        {
            return BotAbilityType.Dash;
        }
    }
}