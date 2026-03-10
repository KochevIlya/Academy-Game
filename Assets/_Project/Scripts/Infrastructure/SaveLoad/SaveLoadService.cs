using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.PersistentProgress;
using _Project.Scripts.Infrastructure.PersistentProgress.Data;
using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit._Data;
using _Project.Scripts.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Infrastructure.SaveLoad
{
    public class SaveLoadService : ISaveLoadService
    {
        private readonly IGameFactory _gameFactory;
        private readonly List<ISaveable> _saveables = new List<ISaveable>();
        private readonly HackingService _hackingService;
    
        private readonly string _path = Path.Combine(Application.persistentDataPath, "save.json");

        public SaveLoadService(IGameFactory gameFactory, HackingService hackingService)
        {
            _gameFactory = gameFactory;
            _hackingService = hackingService;
        }

        public void Save()
        {
            Debug.Log("In SaveLoadService Save()");
            var levelData = new LevelData();

            foreach (var unit in _saveables)
            {
                var data = unit.GetSaveData();
                levelData.enemies.Add(data);
            }
            var allZones = Object.FindObjectsOfType<CombatZone>();
            foreach (var zone in allZones)
            {
                var zoneData = new CombatZoneSaveData
                {
                    ZoneId = zone.GetComponent<EntityIdentifier>().ID,
                    
                    ActiveUnitIds = zone.GetActiveUnits().Select(u => u.Id).ToList()
                };
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

            var toDestroy = _saveables.ToList();
            foreach (var unit in toDestroy)
            {
                if (unit is GameUnit gameUnit) Object.Destroy(gameUnit.gameObject);
            }
            _saveables.Clear();

            var sceneRegistry = Object.FindObjectsOfType<EntityIdentifier>()
                .ToDictionary(x => x.ID, x => x.gameObject);
            Dictionary<string, GameUnit> spawnedUnits = new Dictionary<string, GameUnit>();
            
            
            
            foreach (var enemyData in levelData.enemies)
            {
                GameUnit unit = await _gameFactory.RestoreGameUnit(enemyData);
        
                spawnedUnits.Add(unit.Id, unit);
        
                Register(unit);
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
                }
            }
        
            Debug.Log("[SaveLoadService] Загрузка и стыковка завершены.");
        }

        public void Register(ISaveable saveable)
        {
            if (!_saveables.Contains(saveable))
                _saveables.Add(saveable);
        }

        public void Unregister(ISaveable saveable)
        {
            if (_saveables.Contains(saveable))
                _saveables.Remove(saveable);
        }
    }
}