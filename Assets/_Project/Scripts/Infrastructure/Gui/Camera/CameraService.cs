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

    private Transform _target;
    

    UnityEngine.Camera ICameraService.Camera => _camera;

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

    void Update()
    {
        if (_target == null) return;

        Vector3 origin = _camera.transform.position;
        Vector3 direction = (_target.position - origin).normalized;
        float distance = Vector3.Distance(origin, _target.position);

        RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance);

        HashSet<Renderer> currentHits = new();

        foreach (var hit in hits)
        {
            if (hit.transform == _target) continue;

            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                foreach (var mat in rend.materials)
                {
                    if (mat.HasProperty("_Alpha"))
                    {
                        currentHits.Add(rend);
                        float current = mat.GetFloat("_Alpha");
                        mat.SetFloat("_Alpha", Mathf.MoveTowards(current, minAlpha, Time.deltaTime * alphaSpeed));
                    }
                }
            }
        }

        foreach (var rend in FindObjectsOfType<Renderer>())
        {
            if (!currentHits.Contains(rend))
            {
                foreach (var mat in rend.materials)
                {
                    if (mat.HasProperty("_Alpha"))
                    {
                        float current = mat.GetFloat("_Alpha");
                        mat.SetFloat("_Alpha", Mathf.MoveTowards(current, 1, Time.deltaTime * alphaSpeed));
                    }
                }
            }
        }

        Debug.DrawLine(origin, _target.position, Color.green);
    }
  }
}