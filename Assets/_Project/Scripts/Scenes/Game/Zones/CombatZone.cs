using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using _Project.Scripts.Infrastructure.SaveLoad;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Data;
using _Project.Scripts.Scenes.Game.Unit.Components.Spawner;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;
using _Project.Scripts.Scenes.Game.Hacking.Terminal;
using Zenject;

public class CombatZone : MonoBehaviour, IZoneSaveable
    {
        [SerializeField] private List<UnitSpawner> _mySpawners;
        public List<GameUnit> _activeUnits = new List<GameUnit>();
        [SerializeField] private List<TerminalSpawner> _myTerminalSpawners;
        public List<HackingTerminal> _activeTerminals = new List<HackingTerminal>();
        private bool _isAlarmActive = false;
        private CompositeDisposable _disposables = new CompositeDisposable();
        private int _botsCount = 0;

        private readonly Subject<int> _unitCountSubject = new Subject<int>();
        public IObservable<int> UnitCountChanged => _unitCountSubject;

        private readonly Subject<bool> _battleStateSubject = new Subject<bool>();
        public IObservable<bool> BattleStateChanged => _battleStateSubject;

        public bool IsBattleActive => _isAlarmActive;
        [Inject] HackingService  _hackingService;
        [Inject] InputControllsFactory _inputControllsFactory;
        [Inject] private ISaveLoadService _saveLoadService;
        private int _hackingAttempts = 0;
        private HackingTerminal _terminal;
        [Inject]
        public void Construct(ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
        }
        

        public int BotsCount => _botsCount;

        public List<GameUnit> GetActiveUnits()
        {
            return _activeUnits;
        }
        
        public void InitializeZone()
        {
            _saveLoadService.RegisterZone(this);
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
            _unitCountSubject.OnNext(_activeUnits.Count);

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
                    _unitCountSubject.OnNext(_activeUnits.Count);
                    CheckUnitReturn(unit);
                })
                .AddTo(unit);
            unit.OnUnitHacked
                .Subscribe(_ => CheckLastSurvivor())
                .AddTo(unit);
            _botsCount++;
        }

        private void CheckUnitReturn(GameUnit unit)
        {
            if (unit.IsUnderControl)
            {
                _hackingService.ReturnToOriginalBody();
                _hackingService.StopBattle();
            }

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
        public void ActivateAggroOnUnit(GameUnit unit)
        {
            if (unit == null || unit.IsUnderControl) return;

            var player = _activeUnits.FirstOrDefault(u => u != null && u.IsUnderControl);
            if (player == null) return;

            var aggro = _inputControllsFactory.ChangeAggressiveControls(unit, player, _terminal);
            unit.UpdateControls(aggro);
            Debug.Log($"<color=red>[CombatZone {name}]</color> Юнит {unit.name} переведён в Aggro режим. Цель: {player.name}");
        }

        private void ActivateAggro(GameUnit target)
        {
            _isAlarmActive = true;
            _battleStateSubject.OnNext(true);

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
            _battleStateSubject.OnNext(false);
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
            return _base + 1 * (_hackingAttempts - 1) > 12 ? 12 : _base + 1 * (_hackingAttempts - 1);
        }

        public void ClearState()
        {
            _activeUnits.Clear();
            _botsCount = 0;
        }


        public CombatZoneSaveData GetSaveData()
        {
            return new CombatZoneSaveData
            {
                ZoneId = GetComponent<EntityIdentifier>().ID,
                Attempts = _hackingAttempts,
                ActiveUnitIds = GetActiveUnits().Select(u => u.Id).ToList()
            };
        }

        public void LoadFromData(CombatZoneSaveData data)
        {
            _hackingAttempts = data.Attempts;
            
        }

        public void OnDestroy()
        {
            _saveLoadService.UnregisterZone(this);
        }
    }