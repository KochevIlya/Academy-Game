using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICursorService
{
    void Register(CursorController controller);
    void SetVisible(bool isVisible);
    void SetCrosshairCursor();
    void SetDefaultCursor();
    void SetLockState(bool isLocked);
    
 
}
