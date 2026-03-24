using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMenuActionsService
{
    void SaveGame();
    void LoadGame();
    void RestartLevel();
    void ExitGame();
}


