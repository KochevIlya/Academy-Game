using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Shoot;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Components.Health;
using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    [Header("Настройки ближнего боя")]
        [SerializeField] private float _attackRadius = 2.5f;
        [SerializeField] private float _attackAngle = 90f;
        [SerializeField] private LayerMask _targetLayer;

        [Header("Визуализация (Mesh)")]
        [SerializeField] private MeshFilter _coneMeshFilter;
        [SerializeField] private float _visualDuration = 0.2f;
        [SerializeField] private int _segments = 20;

        private float _currentTime;
        private Mesh _mesh;
        private Coroutine _visualRoutine;

        private void Awake()
        {
            _mesh = new Mesh();
            if (_coneMeshFilter != null) _coneMeshFilter.mesh = _mesh;
            
        }

        private void Update()
        {
            if (_currentTime < WeaponData.CoolDown)
                _currentTime += Time.deltaTime;
        }

        public override void Shoot(Vector2 shootMousePosition, GameUnit unit)
        {
            if (_currentTime >= WeaponData.CoolDown)
            {
                if (_coneMeshFilter != null)
                {
                    if (_visualRoutine != null) StopCoroutine(_visualRoutine);
                    _visualRoutine = StartCoroutine(DrawConeMesh(unit.transform));
                }

                PerformAttack(unit);

                _currentTime = 0f;
            }
        }

        private void PerformAttack(GameUnit unit)
        {
            Collider[] hits = Physics.OverlapSphere(unit.transform.position, _attackRadius, _targetLayer);
            List<GameObject> damagedObjects = new List<GameObject>();

            foreach (var hit in hits)
            {
                if (!hit.isTrigger) continue;
                if (hit.gameObject == unit.gameObject) continue;
                if (damagedObjects.Contains(hit.gameObject)) continue;

                Vector3 dirToTarget = (hit.transform.position - unit.transform.position).normalized;
                dirToTarget.y = 0;

                if (Vector3.Angle(unit.transform.forward, dirToTarget) <= _attackAngle / 2f)
                {
                    if (hit.TryGetComponent(out Health targetHealth))
                    {
                        targetHealth.TakeDamage(WeaponData.Damage);
                        damagedObjects.Add(hit.gameObject);
                        Debug.Log($"[MeleeWeapon] {unit.name} нанес урон {hit.name}");
                    }
                }
            }
        }

        private IEnumerator DrawConeMesh(Transform origin)
        {
            _coneMeshFilter.gameObject.SetActive(true);
            float elapsed = 0f;

            while (elapsed < _visualDuration)
            {
                GenerateConeMesh(origin);
                elapsed += Time.deltaTime;
                yield return null;
            }

            _mesh.Clear();
            _coneMeshFilter.gameObject.SetActive(false);
        }

        private void GenerateConeMesh(Transform origin)
        {
            _mesh.Clear();
            int numVertices = _segments + 2;
            Vector3[] vertices = new Vector3[numVertices];
            int[] triangles = new int[_segments * 3];

            vertices[0] = _coneMeshFilter.transform.InverseTransformPoint(origin.position + Vector3.up * 0.1f);

            float angleStep = _attackAngle / _segments;
            float currentAngle = -_attackAngle / 2f;

            for (int i = 0; i <= _segments; i++)
            {
                Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
                Vector3 dir = rotation * origin.forward;
                Vector3 worldPoint = origin.position + (dir * _attackRadius) + Vector3.up * 0.1f;
                vertices[i + 1] = _coneMeshFilter.transform.InverseTransformPoint(worldPoint);

                if (i < _segments)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
                currentAngle += angleStep;
            }

            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }
}
