using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Components.Spawner;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using _Project.Scripts.Scenes.Game.Hacking.Terminal;
using Zenject;

public class CombatZone : MonoBehaviour
    {
        [SerializeField] private List<UnitSpawner> _mySpawners;
        private List<GameUnit> _activeUnits = new List<GameUnit>();
        [SerializeField] private List<TerminalSpawner> _myTerminalSpawners;
        private List<HackingTerminal> _activeTerminals = new List<HackingTerminal>();
        private bool _isAlarmActive = false;
        private CompositeDisposable _disposables = new CompositeDisposable();
        [Inject] HackingService  _hackingService;
        
        public void InitializeZone()
        {
            foreach (var spawner in _mySpawners)
            {
                if (spawner.SpawnedUnit != null)
                {
                    RegisterUnit(spawner.SpawnedUnit);
                }
            }
            
            foreach (var tSpawner in _myTerminalSpawners)
            {
                if (tSpawner.SpawnedTerminalObject != null)
                {
                    var terminal = tSpawner.SpawnedTerminalObject.GetComponentInChildren<HackingTerminal>();

                    if (terminal != null)
                    {
                        _activeTerminals.Add(terminal);
                        Debug.Log($"<color=green>[CombatZone {name}]</color> Терминал <b>{terminal.name}</b> успешно подключен к зоне.");
                    }
                }
            }
            
            _hackingService.OnHackingProcessStarted
                .Subscribe(_ => 
                {
                    foreach (var terminal in _activeTerminals)
                    {
                        if (terminal._isActive == true) ActivateWalk();
                    }
                })
                .AddTo(_disposables);
        }

        private void RegisterUnit(GameUnit unit)
        {
            _activeUnits.Add(unit);

            unit.Health.OnDamageTaken
                .Subscribe(_ => CheckAlarm())
                .AddTo(unit);

            unit.Health.Die
                .Subscribe(_ => {
                    _activeUnits.Remove(unit); 
                    CheckLastSurvivor();
                })
                .AddTo(unit);
            unit.OnUnitHacked
                .Subscribe(_ => CheckLastSurvivor())
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
        private void CheckLastSurvivor()
        {
            var remainingBots = _activeUnits.Where(u => u != null).ToList();
            
            if (remainingBots.Count == 1)
            {
                var lastBot = remainingBots[0];
        
                if (lastBot.IsUnderControl)
                {
                    Debug.Log($"<color=yellow>ZONE: Игрок остался один в теле {lastBot.name}. Самоуничтожение носителя.</color>");
            
                    lastBot.Health.TakeDamage(999999); 
                    Observable.TimerFrame(1).Subscribe(_ => _hackingService.ReturnToOriginalBody());
                }
            }
        }
        private void ActivateAggro(GameUnit target)
        {
            _isAlarmActive = true;
            
            target.Health.Die
                .Take(1)
                .Subscribe(_ => ActivateWalk())
                .AddTo(_disposables);
            
            Debug.Log($"<color=red>ЗОНА {name}: ТРЕВОГА! Цель: {target.name}</color>");

            foreach (var bot in _activeUnits)
            {
                if (bot == null || bot.IsUnderControl) continue;
                
                var aggro = new AggroInputControls(bot, target);
                bot.UpdateControls(aggro);
            }
        }

        private void ActivateWalk()
        {
            _isAlarmActive = false;
            Debug.Log($"<color=green>ЗОНА {name}: цель: уничтожена</color>");

            foreach (var bot in _activeUnits)
            {
                if (bot == null || bot.IsUnderControl) continue;
                
                var walk = new WalkerInputControls(bot);
                bot.UpdateControls(walk);
            }
        }
        
        private void DeactivateZoneAlert()
        {
            _isAlarmActive = false;
            Debug.Log("<color=green>ZONE: Цель уничтожена, возвращаемся в патруль.</color>");

            foreach (var bot in _activeUnits)
            {
                if (bot == null || bot.IsUnderControl) continue;
        
                bot.DisableControl(new PatrolInputControls(bot)); 
            }
        }
    }