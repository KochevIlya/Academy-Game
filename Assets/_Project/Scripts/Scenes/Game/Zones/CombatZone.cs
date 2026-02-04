using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Components.Spawner;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;

public class CombatZone : MonoBehaviour
    {
        [SerializeField] private List<UnitSpawner> _mySpawners;
        private List<GameUnit> _activeUnits = new List<GameUnit>();
        private bool _isAlarmActive = false;
        private CompositeDisposable _disposables = new CompositeDisposable();

        public void InitializeZone()
        {
            foreach (var spawner in _mySpawners)
            {
                if (spawner.SpawnedUnit != null)
                {
                    RegisterUnit(spawner.SpawnedUnit);
                }
            }
        }

        private void RegisterUnit(GameUnit unit)
        {
            _activeUnits.Add(unit);

            unit.Health.OnDamageTaken
                .Subscribe(_ => CheckAlarm())
                .AddTo(unit);

            unit.Health.Die
                .Subscribe(_ => _activeUnits.Remove(unit))
                .AddTo(unit);
        }

        private void CheckAlarm()
        {
            if (_isAlarmActive) return;

            var player = _activeUnits.FirstOrDefault(u => u != null && u.IsUnderControl);

            if (player != null)
            {
                ActivateAggro(player);
            }
        }

        private void ActivateAggro(GameUnit target)
        {
            _isAlarmActive = true;
            
            target.Health.Die
                .Take(1)
                .Subscribe(_ => DeactivateZoneAlert())
                .AddTo(_disposables);
            
            Debug.Log($"<color=red>ЗОНА {name}: ТРЕВОГА! Цель: {target.name}</color>");

            foreach (var bot in _activeUnits)
            {
                if (bot == null || bot.IsUnderControl) continue;
                
                var aggro = new AggroInputControls(bot, target);
                bot.UpdateControls(aggro);
            }
        }
        private void DeactivateZoneAlert()
        {
            _isAlarmActive = false;
            Debug.Log("<color=green>ZONE: Цель уничтожена, возвращаемся в патруль.</color>");

            foreach (var bot in _activeUnits)
            {
                if (bot == null || bot.IsUnderControl) continue;
        
                bot.DisableControl(new DummyInputControls()); 
            }
        }
    }