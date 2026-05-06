using _Project.Scripts.Scenes.Game.Unit;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;

namespace _Project.Scripts.Infrastructure.Gui.Camera
{
  public sealed class CameraService : MonoBehaviour, ICameraService
  {
    [SerializeField] private UnityEngine.Camera _camera;
    [SerializeField] private CinemachineCamera _cameraZoomIn;
    [SerializeField] private float alphaSpeed = 1f;
    [SerializeField] private float minAlpha = 0f;
    
    private static readonly int AlphaProperty = Shader.PropertyToID("_Alpha");
    
    private Transform _target;
    private Dictionary<Renderer, List<Material>> _activeFaders = new();
    private List<Renderer> _toRemove = new();
    private CinemachineFollow _followComponent;
    private float _defaultDistance;
    private float _targetDistance;
    [SerializeField] private float _zoomOutCoef = 1.5f;
    [SerializeField] private float _zoomSpeed = 10f;
    
    UnityEngine.Camera ICameraService.Camera => _camera;

    void Awake()
    {
        _followComponent = _cameraZoomIn.GetComponent<CinemachineFollow>();
        _defaultDistance = _cameraZoomIn.Lens.OrthographicSize;
        _targetDistance = _defaultDistance;
    }
    
    void ICameraService.Init()
    {

    }

    public void SetPoint(Transform point)
    {
        _cameraZoomIn.Follow = point;
        _target = point;
    }
    public void SetTarget(GameUnit unit)
    {
        if (unit == null)
        {
            _cameraZoomIn.Follow = null;
            _target = null;
            return;
        }
        
        _cameraZoomIn.Follow = unit.transform;
        _target = unit.transform;
    }

    void ICameraService.Cleanup()
    {
      _cameraZoomIn.Follow = null;
      _target = null;
    }

    public void ZoomOut()
    {
        _targetDistance = _defaultDistance * _zoomOutCoef;
    } 
    public void ResetZoom()
    {
        _targetDistance = _defaultDistance;
    }
    void Update()
    {
        HandleZoom();
        
        if (_target == null)
        {
            ResetAllFaders();
            return;
        }
        HandleFadeLogic();
    }

    private void HandleFadeLogic()
    {
        Vector3 origin = _camera.transform.position;
        Vector3 direction = (_target.position - origin).normalized;
        float distance = Vector3.Distance(origin, _target.position);
        
        RaycastHit[] hits = Physics.SphereCastAll(origin, 0.5f, direction, distance);
        HashSet<Renderer> currentHits = new();
        foreach (var hit in hits)
        {
            if (hit.transform == _target || _target.IsChildOf(hit.transform)) continue;

            Renderer[] renderers = hit.transform.GetComponentsInChildren<Renderer>();

            foreach (var rend in renderers)
            {
                ProcessRenderer(rend, currentHits, minAlpha);
            }
        }
        UpdateAlphaTransitions(currentHits);
    }

    private void ProcessRenderer(Renderer rend, HashSet<Renderer> currentHits, float targetAlpha)
    {
        currentHits.Add(rend);
        if (!_activeFaders.ContainsKey(rend))
        {
            _activeFaders.Add(rend, new List<Material>(rend.materials));
        }
        FadeMaterials(_activeFaders[rend], targetAlpha);
    }

    private void UpdateAlphaTransitions(HashSet<Renderer> currentHits)
    {
        _toRemove.Clear();
        foreach (var pair in _activeFaders)
        {
            Renderer rend = pair.Key;
            if (!currentHits.Contains(rend))
            {
                bool isDone = FadeMaterials(pair.Value, 1.0f);
                if (isDone) _toRemove.Add(rend);
            }
        }

        foreach (var rend in _toRemove)
        {
            _activeFaders.Remove(rend);
        }
    }

    private bool FadeMaterials(List<Material> materials, float target)
    {
        bool allFinished = true;
        foreach (var mat in materials)
        {
            if (mat.HasProperty(AlphaProperty))
            {
                float current = mat.GetFloat(AlphaProperty);
                float next = Mathf.MoveTowards(current, target, Time.deltaTime * alphaSpeed);
                mat.SetFloat(AlphaProperty, next);
                if (!Mathf.Approximately(next, target)) allFinished = false;
            }
        }

        return allFinished;
    }

    private void ResetAllFaders()
    {
        if (_activeFaders.Count == 0) return;
        foreach (var pair in _activeFaders) FadeMaterials(pair.Value, 1.0f);
        _activeFaders.Clear();
    }
    
    private void HandleZoom()
    {
        if (_cameraZoomIn == null) return;

        if (_cameraZoomIn.Lens.Orthographic)
        {
            var lens = _cameraZoomIn.Lens;
        
            
            float currentSize = lens.OrthographicSize;
            float nextSize = Mathf.Lerp(currentSize, _targetDistance, Time.unscaledDeltaTime * _zoomSpeed);
        
            lens.OrthographicSize = nextSize;
        
            _cameraZoomIn.Lens = lens;
        }
    }
  }
}