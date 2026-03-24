using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStorageService
{
    void SaveData(string key, string jsonData);
    string LoadData(string key);
}
