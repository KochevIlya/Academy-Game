using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorService : ICursorService
{
    private CursorController _controller;

    public void Register(CursorController controller)
    {
        _controller = controller;
        SetCrosshairCursor();
    }

    public void SetVisible(bool isVisible)
    {
        if (_controller != null)
        {
            _controller.SetVisible(isVisible);
        }
    }
    public void SetCrosshairCursor()
    {
        _controller?.SetCrosshairCursor();
    }

    public void SetDefaultCursor()
    {
        _controller?.SetDefaultCursor();
    }
    public void SetLockState(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
