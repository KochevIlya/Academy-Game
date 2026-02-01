using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SimpleOutline : MonoBehaviour
{
    [SerializeField] private Color outlineColor = Color.cyan;
    [SerializeField] private float outlineWidth = 2f;

    private Renderer[] _renderers;
    private Material _maskMat;
    private Material _fillMat;
    private bool _isActive = false;

    void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _maskMat = new Material(Shader.Find("Custom/OutlineMask"));
        _fillMat = new Material(Shader.Find("Custom/OutlineFill"));
        _fillMat.SetColor("_OutlineColor", outlineColor);
        _fillMat.SetFloat("_OutlineWidth", outlineWidth);
    }

    public void SetHighlight(bool active)
    {
        if (_isActive == active) return;
        _isActive = active;

        if (_renderers == null) return;

        foreach (var r in _renderers)
        {
            if (r == null) continue;

            var mats = r.materials.ToList();

            if (active)
            {
                if (!mats.Any(m => m.shader.name == "Custom/OutlineMask"))
                    mats.Add(_maskMat);
                if (!mats.Any(m => m.shader.name == "Custom/OutlineFill"))
                    mats.Add(_fillMat);
            }
            else
            {
                mats.RemoveAll(m => m.shader.name == "Custom/OutlineMask" || 
                                    m.shader.name == "Custom/OutlineFill");
            }

            r.materials = mats.ToArray();
        }
    }
}