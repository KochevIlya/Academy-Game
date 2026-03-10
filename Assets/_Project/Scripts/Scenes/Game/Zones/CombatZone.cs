using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Data;
using _Project.Scripts.Scenes.Game.Unit.Components.Spawner;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using _Project.Scripts.Scenes.Game.Hacking.Terminal;
using Zenject;

public class CombatZone : MonoBehaviour
    {
        [SerializeField] private List<UnitSpawner> _mySpawners;
        public List<GameUnit> _activeUnits = new List<GameUnit>();
        [SerializeField] private List<TerminalSpawner> _myTerminalSpawners;
        public List<HackingTerminal> _activeTerminals = new List<HackingTerminal>();
        private bool _isAlarmActive = false;
        private CompositeDisposable _disposables = new CompositeDisposable();
        private int _botsCount = 0;
        [Inject] HackingService  _hackingService;
        [Inject] InputControllsFactory _inputControllsFactory;
        private int _hackingAttempts = 0;
        private HackingTerminal _terminal;


        public List<GameUnit> GetActiveUnits()
        {
            return _activeUnits;
        }
        
        public void InitializeZone()
        {
            foreach (var spawner in _mySpawners)
            {
                if (spawner.SpawnedUnit != null)
                {
                    RegisterUnit(spawner.SpawnedUnit);
                    _botsCount++;
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
                        
                        if (terminal._isActive == true)
                        {
                            _hackingService.SetCurrentZoneContext(this);
                            _hackingAttempts++;
                            if(_botsCount == 0)
                                _hackingService.RequestCancel();
                            else
                            {
                                ActivateWalk(terminal);
                                _terminal =  terminal;
                            }
                        }
                    }
                })
                .AddTo(_disposables);
            
        }

        public void RegisterUnit(GameUnit unit)
        {
            _activeUnits.Add(unit);

            unit.Health.OnDamageTaken
                .Subscribe(_ => 
                    {
                        
                        if (unit.Data.behaviourType == UnitBehaviourType.Melee && !unit.IsUnderControl)
                        {
                            var target = _activeUnits.FirstOrDefault(u => u != null && u.IsUnderControl);
                            if (target != null)
                            {
                                unit.UpdateControls(_inputControllsFactory.ChangeALlAggressiveControls(unit, target));
                            }

                            CheckAlarm();
                        }
                        else
                        {
                            CheckAlarm();
                        }
                    })
                .AddTo(unit);

            unit.Health.Die
                .Subscribe(_ => {
                    _activeUnits.Remove(unit); 
                    CheckLastSurvivor();
                    _botsCount--;
                    CheckUnitReturn(unit);
                })
                .AddTo(unit);
            unit.OnUnitHacked
                .Subscribe(_ => CheckLastSurvivor())
                .AddTo(unit);
            
        }

        private void CheckUnitReturn(GameUnit unit)
        {
            if (unit.IsUnderControl)
                _hackingService.ReturnToOriginalBody();
            
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
                .Subscribe(_ => ActivateWalk(_terminal))
                .AddTo(_disposables);
            
            Debug.Log($"<color=red>ЗОНА {name}: ТРЕВОГА! Цель: {target.name}</color>");

            foreach (var bot in _activeUnits)
            {
                if (bot == null || bot.IsUnderControl || bot.Data.behaviourType == UnitBehaviourType.Melee) continue;
                
                var aggro = _inputControllsFactory.ChangeAggressiveControls(bot, target, _terminal);
                bot.UpdateControls(aggro);
            }
        }

        private void ActivateWalk(HackingTerminal terminal)
        {
            _isAlarmActive = false;
            Debug.Log($"<color=green>ЗОНА {name}: цель: уничтожена</color>");

            foreach (var bot in _activeUnits)
            {
                if (bot == null || bot.IsUnderControl) continue;
                
                var walk = new WalkerInputControls(bot, terminal);
                bot.UpdateControls(walk);
            }
        }
        public int GetNextSequenceLength(int _base)
        {
            return _base + 2 * (_hackingAttempts - 1) > 12 ? 12 : _base + 2 * (_hackingAttempts - 1);
        }

        public void ClearState()
        {
            _activeUnits.Clear();
        }
        
        
    }