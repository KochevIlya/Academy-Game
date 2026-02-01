using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairService : ICrosshairService
{
    private CrosshairController _controller;

    public void Register(CrosshairController controller)
    {
        _controller = controller;
    }

    public void SetVisible(bool isVisible)
    {
        if (_controller != null)
        {
            _controller.SetVisible(isVisible);
        }
    }
}
