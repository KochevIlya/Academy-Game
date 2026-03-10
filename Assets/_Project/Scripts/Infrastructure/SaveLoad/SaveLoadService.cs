using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.PersistentProgress;
using _Project.Scripts.Infrastructure.PersistentProgress.Data;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Scenes.Game.Hacking.Terminal;
using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Infrastructure.States;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Data;
using _Project.Scripts.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Infrastructure.SaveLoad
{
    public class SaveLoadService : ISaveLoadService
    {
        private readonly IGameFactory _gameFactory;
        private readonly List<IUnitSaveable> _unitSaveables = new List<IUnitSaveable>();
        private readonly List<ITerminalSaveable> _terminalSaveables = new List<ITerminalSaveable>();
        private readonly List<IZoneSaveable> _zoneSaveables = new List<IZoneSaveable>();
        private readonly HackingService _hackingService;
        private readonly string _path = Path.Combine(Application.persistentDataPath, "save.json");
        public SaveLoadService(
            IGameFactory gameFactory,
            HackingService hackingService
            )
        {
            _gameFactory = gameFactory;
            _hackingService = hackingService;
        }

        public void Save()
        {
            Debug.Log("In SaveLoadService Save()");
            var levelData = new LevelData();

            foreach (var unit in _unitSaveables)
            {
                var data = unit.GetSaveData();
                levelData.enemies.Add(data);
            }

            foreach (var terminal in _terminalSaveables)
            {
                var data = terminal.GetSaveData();
                levelData.triggers.Add(data);
            }
            
            foreach (var zone in _zoneSaveables)
            {
                var zoneData = zone.GetSaveData();
                
                levelData.zones.Add(zoneData);
            }
            string json = JsonUtility.ToJson(levelData, true); 
            File.WriteAllText(_path, json);
            Debug.Log($"[SaveLoadService] Сохранено в: {_path}");
        }

        public async UniTask LoadAsync()
        {
            _hackingService.ClearState();
            Debug.Log("In SaveLoadService LoadAsync()");
            if (!File.Exists(_path))
            {
                Debug.LogWarning("Файл сохранения не найден!");
                return;
            }

            string json = File.ReadAllText(_path);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);

            var toDestroy = _unitSaveables.ToList();
            foreach (var unit in toDestroy)
            {
                if (unit is GameUnit gameUnit) Object.Destroy(gameUnit.gameObject);
            }

            _unitSaveables.Clear();

            var sceneRegistry = Object.FindObjectsOfType<EntityIdentifier>()
                .ToDictionary(x => x.ID, x => x.gameObject);
            Dictionary<string, GameUnit> spawnedUnits = new Dictionary<string, GameUnit>();



            foreach (var enemyData in levelData.enemies)
            {
                GameUnit unit = await _gameFactory.RestoreGameUnit(enemyData);

                spawnedUnits.Add(unit.Id, unit);

                RegisterUnit(unit);
            }
            
            
            foreach (var zoneSaveData in levelData.zones)
            {
                if (sceneRegistry.TryGetValue(zoneSaveData.ZoneId, out var zoneObj))
                {
                    CombatZone zone = zoneObj.GetComponent<CombatZone>();
                    zone.ClearState();
                    foreach (var unitId in zoneSaveData.ActiveUnitIds)
                    {
                        if (spawnedUnits.TryGetValue(unitId, out var unit))
                        {
                            zone.RegisterUnit(unit);
                        }
                    }
                    zone.LoadFromData(zoneSaveData);
                }
            }

            foreach (var triggerData in levelData.triggers)
            {
                var terminal = _terminalSaveables.FirstOrDefault(t => t.GetSaveData().Id == triggerData.Id);

                if (terminal != null)
                {
                    terminal.LoadFromData(triggerData);
                }
                else
                {
                    Debug.LogWarning($"[SaveLoad] Терминал с ID {triggerData.Id} не найден среди зарегистрированных!");
                }

            }
            Debug.Log("[SaveLoadService] Загрузка и стыковка завершены.");
        }

        public void RegisterUnit(IUnitSaveable saveable)
        {
            if (!_unitSaveables.Contains(saveable))
                _unitSaveables.Add(saveable);
        }

        public void UnregisterUnit(IUnitSaveable saveable)
        {
            if (_unitSaveables.Contains(saveable))
                _unitSaveables.Remove(saveable);
        }

        public void RegisterTerminal(ITerminalSaveable saveable)
        {
            if(!_terminalSaveables.Contains(saveable))
                _terminalSaveables.Add(saveable);
        }

        public void UnregisterTerminal(ITerminalSaveable saveable)
        {
            if (_terminalSaveables.Contains(saveable))
                _terminalSaveables.Remove(saveable);
        }

        public void RegisterZone(IZoneSaveable saveable)
        {
            if(!_zoneSaveables.Contains(saveable))
                _zoneSaveables.Add(saveable);
        }

        public void UnregisterZone(IZoneSaveable saveable)
        {
            if(_zoneSaveables.Contains(saveable))
                _zoneSaveables.Remove(saveable);
        }

        public bool HasSaveFile() => File.Exists(_path);
    }
}