using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit.Components.Health;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    private float speed = 5f;
    private float explosionRadius = 3f;
    private int damage = 20;
    private float fuseTime = 2f;
    private LayerMask enemyLayer;

    private Vector3 _targetPosition;
    private bool _exploded = false;
    [SerializeField] private GrenadeExplosionEffect _explosionPrefab;
    [SerializeField] private float _visualDuration = 0.5f;
    public void Setup(Vector3 targetPosition, int damage, float radius, float fuseTime, float speed)
    {
        _targetPosition = targetPosition;
        this.damage = damage;
        this.explosionRadius = radius;
        this.fuseTime = fuseTime;
        this.speed = speed;
    }

    private void Start()
    {
        StartCoroutine(FlyAndExplode());
    }

    private IEnumerator FlyAndExplode()
    {
        while (Vector3.Distance(transform.position, _targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(fuseTime);
        
        Explode();
    }

    private void Explode()
    {
        if (_exploded) return;
        _exploded = true;

        if (_explosionPrefab != null)
        {
            var effect = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            effect.Initialize(explosionRadius, _visualDuration);
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider.CompareTag("HitBox")) continue;
            var health = hitCollider.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
        
        Debug.Log($"BOOM! Radiused: {explosionRadius}, Found colliders: {hitColliders.Length}");
        Destroy(gameObject);
    }
}