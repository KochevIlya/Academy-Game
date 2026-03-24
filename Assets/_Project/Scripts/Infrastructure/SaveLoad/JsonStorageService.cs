using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonStorageService : IStorageService
{
    private readonly string _savePath = Application.persistentDataPath;

    public void SaveData(string key, string jsonData)
    {
        File.WriteAllText(Path.Combine(_savePath, key + ".json"), jsonData);
    }

    public string LoadData(string key)
    {
        string path = Path.Combine(_savePath, key + ".json");
        return File.Exists(path) ? File.ReadAllText(path) : null;
    }
}
