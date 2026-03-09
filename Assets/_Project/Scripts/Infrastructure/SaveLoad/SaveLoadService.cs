using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.PersistentProgress;
using _Project.Scripts.Infrastructure.PersistentProgress.Data;
using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Unit;
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
    
        private readonly string _path = Path.Combine(Application.persistentDataPath, "save.json");

        public SaveLoadService(IGameFactory gameFactory) => _gameFactory = gameFactory;

        public void Save()
        {
            Debug.Log("In SaveLoadService Save()");
            var levelData = new LevelData();

            foreach (var saveable in _saveables)
            {
                levelData.enemies.Add(saveable.GetSaveData());
            }
            
            string json = JsonUtility.ToJson(levelData, true); 
            File.WriteAllText(_path, json);
            Debug.Log($"[SaveLoadService] Сохранено в: {_path}");
        }

        public async UniTask LoadAsync()
        {
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

            foreach (var data in levelData.enemies)
            {
                await _gameFactory.RestoreGameUnit(data);
            }
        
            Debug.Log("[SaveLoadService] Загрузка завершена.");
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