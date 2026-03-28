using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProgressService
{
    LevelData Progress { get; set; }
    bool IsFirstStart { get; set; }
    void Save();

    void Load();
    public bool HasSaveFile();
}
