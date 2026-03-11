using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Configs;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Abilities
{
    public class DashAbility : MonoBehaviour, IAbility
    {
        [Inject] private readonly UserInputControls _input;
        [Inject] private readonly IInputHelper _inputHelper;
        
        private GameUnit _unit;
        private AbilityConfig _abilityConfig;
        private DashSettings _settings;
        private CharacterController _characterController;
        
        private float _timer;
        
        public void Use(Vector3 targetPosition)
        {
            if (!CanUse()) return;
            
            UseAbility().Forget();
        }

        public bool CanUse() => _settings != null && _timer <= 0;

        private async UniTaskVoid UseAbility()
        {
            if (_settings == null || _characterController == null) return;
            
            _timer = _settings.cooldown;
            
            Vector2 mouseScreenPos = _input.MousePosition;
            if (!_inputHelper.ScreenToGroundPosition(mouseScreenPos, _unit.transform.position.y, out Vector3 targetWorldPos))
            {
                return;
            }

            Vector3 direction = targetWorldPos - _unit.transform.position;
            direction.y = 0f;
            direction.Normalize();

            _input.IsBlocked.Value = true;

            float distanceDashed = 0f;

            while (distanceDashed < _settings.distance)
            {
                float step = _settings.speed * Time.deltaTime;
                
                CollisionFlags flags = _characterController.Move(direction * step);
                
                distanceDashed += step;

               if ((flags & CollisionFlags.Sides) != 0)
                {
                    break;
                }

                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            _input.IsBlocked.Value = false;
        }

        public void Initialize(GameUnit unit, AbilityConfig config)
        {
            _unit = unit;
            _abilityConfig = config;
            _characterController = unit.GetComponent<CharacterController>();
            var settings = _abilityConfig.GetSettings(BotAbilityType.Dash) as DashSettings;
            
            if (settings != null)
            {
                _settings = settings;
                _timer = 0f;
            }
            else
            {
                Debug.LogError($"Dash settings not found in {_abilityConfig.name}");
                enabled = false;
            }
        }
        
        private void Update()
        {
            if (_settings != null && _timer > 0)
            {
                _timer -= Time.deltaTime;
            }
        }
        
    }
}