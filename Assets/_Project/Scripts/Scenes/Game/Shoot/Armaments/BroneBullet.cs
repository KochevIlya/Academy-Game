using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Shoot;
using UnityEngine;

public class BroneBullet : Bullet
{
    [Header("Brone Settings")]
    [SerializeField] private float _changeDirectionInterval = 0.02f;
    [SerializeField] private float _randomAngleRange = 150f;
    [SerializeField] private float _lifetimeDuration = 1f;
    [SerializeField] private int _bulletDamage = 5;
    private float _directionTimer;

    private void OnEnable()
    {
        _directionTimer = _changeDirectionInterval;
        _lifeTime = _lifetimeDuration;
        _damage = _bulletDamage;
    }

    private new void Update()
    {
        base.Update(); 

        _directionTimer -= Time.deltaTime;

        if (_directionTimer <= 0)
        {
            ChangeDirectionRandomly();
            _directionTimer = _changeDirectionInterval;
        }
    }

    private void ChangeDirectionRandomly()
    {
        
        Vector3 currentDir = transform.forward;
    
        float randomAngleY = Random.Range(-_randomAngleRange, _randomAngleRange);
    
        Quaternion randomRotation = Quaternion.Euler(0f, randomAngleY, 0f);

        Vector3 newDir = randomRotation * currentDir;
        
        SetDirection(newDir.normalized);
    }
}
