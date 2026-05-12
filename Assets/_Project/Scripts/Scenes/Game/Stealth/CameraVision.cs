using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.StateMachine;
using UnityEngine;
using Zenject;

public class CameraVision : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Transform _eyeAnchor;
    [SerializeField] private MeshFilter _viewMeshFilter;
    [SerializeField] private MeshRenderer _viewMeshRenderer;

    [Header("Настройки обзора")]
    [SerializeField] private float _viewRadius = 5f;
    [Range(0, 360)]
    [SerializeField] private float _viewAngle = 70f;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstacleMask;

    [Header("Настройки обнаружения")]
    [SerializeField] private float _timeToCatch = 2f;
    [SerializeField] private float _detectionDrainRate = 1f;

    [Header("Визуализация")]
    [SerializeField] private int _segments = 50;

    private Mesh _viewMesh;
    private GameObject _owner;
    private float _eyeHeight;
    private float _currentDetectionTime = 0f;
    private bool _isWorking = true;

    private static readonly int _DetectionProgressProp = Shader.PropertyToID("_DetectionProgress");

    [Inject]
    private IGameStateMachine _gameStateMachine;
    
    private void Awake()
    {
        _viewMesh = new Mesh { name = "View Mesh" };
        _viewMeshFilter.mesh = _viewMesh;

        if (_viewMeshRenderer == null)
            _viewMeshRenderer = _viewMeshFilter.GetComponent<MeshRenderer>();

        _owner = transform.parent != null ? transform.parent.gameObject : gameObject;
        _eyeHeight = _eyeAnchor.position.y;
    }

    private void LateUpdate()
    {
        if (_owner == null || _eyeAnchor == null || !_isWorking) return;

        DrawVisionCone();
        FindVisibleTargets();
        UpdateShaderParameters();
    }
    
    private void FindVisibleTargets()
    {
        Vector3 origin = _eyeAnchor.position;
        Collider[] targetsInRadius = Physics.OverlapSphere(origin, _viewRadius, _targetMask);

        bool isPlayerVisibleThisFrame = false;

        foreach (var target in targetsInRadius)
        {
            if (!target.CompareTag("VisionHitbox")) continue;

            Vector3 targetPosition = target.bounds.center;
            Vector3 dirToTarget = (targetPosition - origin).normalized;
            float dstToTarget = Vector3.Distance(origin, targetPosition);

            if (Vector3.Angle(_eyeAnchor.forward, dirToTarget) < _viewAngle / 2f)
            {
                if (!Physics.Raycast(origin, dirToTarget, dstToTarget, _obstacleMask))
                {
                    isPlayerVisibleThisFrame = true;
                    break;
                }
            }
        }

        if (isPlayerVisibleThisFrame)
        {
            _currentDetectionTime += Time.deltaTime;
            if (_currentDetectionTime >= _timeToCatch)
            {
                _gameStateMachine.Enter<GameOverState>();
            }
        }
        else
        {
            _currentDetectionTime -= Time.deltaTime * _detectionDrainRate;
        }

        _currentDetectionTime = Mathf.Clamp(_currentDetectionTime, 0f, _timeToCatch);
    }
    
    private void UpdateShaderParameters()
    {
        if (_viewMeshRenderer != null && _viewMeshRenderer.material != null)
        {
            float progress = _currentDetectionTime / _timeToCatch;
            
            _viewMeshRenderer.material.SetFloat(_DetectionProgressProp, progress);
        }
    }
    
    private void DrawVisionCone()
    {
        _viewMesh.Clear();

        int vertexCount = _segments + 2;
        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];
        int[] triangles = new int[_segments * 3];

        vertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0f); 

        float angleStep = _viewAngle / _segments;
        float currentAngle = -_viewAngle / 2;

        Vector3 eyePosition = transform.position + Vector3.up * _eyeHeight;

        for (int i = 0; i <= _segments; i++)
        {
            Vector3 localDir = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward;
            Vector3 worldDir = transform.TransformDirection(localDir);

            float dist = _viewRadius;

            if (Physics.Raycast(eyePosition, worldDir, out RaycastHit hit, _viewRadius, _obstacleMask))
            {
                dist = hit.distance;
            }

            vertices[i + 1] = localDir * dist;

            float normalizedDist = dist / _viewRadius;
            uvs[i + 1] = new Vector2((float)i / _segments, normalizedDist); 

            if (i < _segments)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
            currentAngle += angleStep;
        }

        _viewMesh.vertices = vertices;
        _viewMesh.uv = uvs;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }
}
