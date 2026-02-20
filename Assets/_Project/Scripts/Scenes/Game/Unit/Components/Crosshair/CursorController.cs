using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit.Controls;
using UniRx;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Texture2D _crosshairTexture;
    
    
    private void Start()
    {
        ActivateCrosshair(); 
    }

    public void Initialize(IInputControls inputControls)
    {
        if (_crosshairTexture != null)
        {
            ActivateCrosshair();
        }
    }
    public void ActivateCrosshair()
    {
        SetCrosshairCursor();
        SetVisible(true);
    }
    public void SetCrosshairCursor()
    {
        if (_crosshairTexture == null) return;
        Vector2 hotspot = new Vector2(_crosshairTexture.width / 2f, _crosshairTexture.height / 2f);
        Cursor.SetCursor(_crosshairTexture, hotspot, CursorMode.Auto);
    }
    public void SetDefaultCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    private void OnDestroy()
    {
        SetDefaultCursor();
    }
    
    public void SetVisible(bool isVisible)
    {
        Cursor.visible = isVisible;
    }
}
