using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Unit.Components.Health;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private int damage = 20;
    [SerializeField] private float fuseTime = 2f;
    [SerializeField] private LayerMask enemyLayer;

    private Vector3 _targetPosition;
    private bool _exploded = false;

    public void Setup(Vector3 targetPosition, int damage, float radius, float fuseTime)
    {
        _targetPosition = targetPosition;
        this.damage = damage;
        this.explosionRadius = radius;
        this.fuseTime = fuseTime;
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

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            
            var health = hitCollider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
        
        Debug.Log("BOOM!");

        Destroy(gameObject);
    }
}