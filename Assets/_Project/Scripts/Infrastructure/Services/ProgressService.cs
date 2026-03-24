using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProgressService : IProgressService {
    public LevelData Progress { get; set; } = new LevelData();
    private string _path = Path.Combine(Application.persistentDataPath, "save.json");

    public void Save() {
        string json = JsonUtility.ToJson(Progress, true);
        File.WriteAllText(_path, json);
    }

    public void Load() {
        if (File.Exists(_path)) {
            string json = File.ReadAllText(_path);
            Progress = JsonUtility.FromJson<LevelData>(json);
        }
    }
}
