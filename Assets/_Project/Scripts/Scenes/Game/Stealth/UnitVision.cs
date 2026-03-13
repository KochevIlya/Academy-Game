using UnityEngine;
using Zenject;
using _Project.Scripts.Infrastructure.StateMachine;
using _Project.Scripts.Scenes.Game.Infrastructure.States;
using _Project.Scripts.Scenes.Game.Unit;

public class UnitVision : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Transform _eyeAnchor;
    [SerializeField] private MeshFilter _viewMeshFilter;

    [Header("Настройки обзора")]
    [SerializeField] private float _viewRadius = 5f;
    [Range(0, 360)]
    [SerializeField] private float _viewAngle = 70f;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstacleMask;

    [Header("Визуализация")]
    [SerializeField] private int _segments = 50;

    private Mesh _viewMesh;
    private GameUnit _owner;
    private float _eyeHeight;
    [Inject]
    private IGameStateMachine _gameStateMachine;

    private void Awake()
    {
        _viewMesh = new Mesh { name = "View Mesh" };
        _viewMeshFilter.mesh = _viewMesh;
        _owner = GetComponentInParent<GameUnit>();
        _eyeHeight = _eyeAnchor.position.y;
        
    }

    private void LateUpdate()
    {
        if (_owner == null || _eyeAnchor == null) return;

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
        Vector3 origin = _eyeAnchor.position;
        Collider[] targetsInRadius = Physics.OverlapSphere(origin, _viewRadius, _targetMask);

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