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

    [Header("Визуализация")]
    [SerializeField] private MeshFilter _viewMeshFilter;
    [SerializeField] private int _segments = 20;

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

        foreach (var target in targetsInRadius)
        {
            GameObject targetGameObject = target.gameObject;
            if (!targetGameObject.CompareTag("VisionHitbox")) continue;
            
            GameUnit unit = targetGameObject.GetComponentInParent<GameUnit>();
            if (unit == null) continue;

            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < _viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, _obstacleMask))
                {
                    Debug.Log("<color=red>ИГРОК ОБНАРУЖЕН!</color>");
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

        for (int i = 0; i <= _segments; i++)
        {
            Vector3 dir = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward;
            
            float dist = _viewRadius;
            if (Physics.Raycast(transform.position, transform.TransformDirection(dir), out RaycastHit hit, _viewRadius, _obstacleMask))
            {
                dist = hit.distance;
            }

            vertices[i + 1] = dir * dist;

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