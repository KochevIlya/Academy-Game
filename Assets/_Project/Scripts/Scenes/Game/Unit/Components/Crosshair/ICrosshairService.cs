using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICrosshairService
{
    void Register(CrosshairController controller);
    void SetVisible(bool isVisible);
}
