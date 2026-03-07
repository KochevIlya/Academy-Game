using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Gui.Service;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Scenes.Game.Infrastructure.States;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;
using Zenject;

public class UnitVision : MonoBehaviour
{
    [Header("Настройки обзора")]
    [SerializeField] private float _viewRadius = 5f;
    [Range(0, 360)]
    [SerializeField] private float _viewAngle = 70f;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _eyeHeight = 1.5f;

    [Header("Визуализация")]
    [SerializeField] private MeshFilter _viewMeshFilter;
    [SerializeField] private int _segments = 50;

    private Mesh _viewMesh;
    private GameUnit _owner;
    [Inject]
    private IGameStateMachine _gameStateMachine;
    
    private void Awake()
    {
        _viewMesh = new Mesh { name = "View Mesh" };
        _viewMeshFilter.mesh = _viewMesh;
        _owner = GetComponentInParent<GameUnit>();
    }

    private void LateUpdate()
    {
        if (_owner == null) return;
        bool isPatrolling = _owner.InputControls is PatrolInputControls || _owner.InputControls is WalkerInputControls;

        if (!isPatrolling)
        {
            _viewMesh.Clear();
            return;
        }
        DrawVisionCone();
        FindVisibleTargets();
    }

    private void FindVisibleTargets()
    {
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, _viewRadius, _targetMask);
        Vector3 eyePosition = transform.position + Vector3.up * _eyeHeight;

        foreach (var target in targetsInRadius)
        {
            Vector3 targetAtEyeLevel = new Vector3(target.transform.position.x, eyePosition.y, target.transform.position.z);

            Vector3 dirToTarget = (targetAtEyeLevel - eyePosition).normalized;
            float dstToTarget = Vector3.Distance(eyePosition, targetAtEyeLevel);

            if (Vector3.Angle(transform.forward, dirToTarget) < _viewAngle / 2)
            {
                if (!Physics.Raycast(eyePosition, dirToTarget, dstToTarget, _obstacleMask) && target.CompareTag("VisionHitbox"))
                {
                    _gameStateMachine.Enter<GameOverState>();
                }
            }
        }
    }

    private void DrawVisionCone()
    {
        _viewMesh.Clear();

        int vertexCount = _segments + 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[_segments * 3];

        vertices[0] = Vector3.zero;

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

            if (i < _segments)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
            currentAngle += angleStep;
        }

        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }
}