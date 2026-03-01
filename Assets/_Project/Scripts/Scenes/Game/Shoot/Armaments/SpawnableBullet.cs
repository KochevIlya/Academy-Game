using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Shoot;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class SpawnableBullet : Bullet
{
    [SerializeField] private AssetReference _bulletToSpawn;
    
    [SerializeField] private float _minSpawnInterval = 0.2f;
    [SerializeField] private float _maxSpawnInterval = 0.8f;
    
    [SerializeField] private float _spawnedSpeed = 15f;
    [SerializeField] private float _spawnedLifeTime = 1f;
    [SerializeField] private float _spawnedCurrentLifeTime = 0f;
    [SerializeField] private int _spawnedDamage = 20; 
    
    private float _spawnTimer;
    
    [Inject] private IGameFactory _gameFactory;
    
    
    
    private void OnEnable()
    {
        SetRandomSpawnTimer();
    }

    private void Update()
    {
        base.Update(); 

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0)
        {
            SpawnSubBullet();
            SetRandomSpawnTimer();
        }
    }
    private void SetRandomSpawnTimer()
    {
        _spawnTimer = Random.Range(_minSpawnInterval, _maxSpawnInterval);
    }
    
    private void SpawnSubBullet()
    {
        if (_bulletToSpawn == null || _gameFactory == null) return;

        _gameFactory.SpawnBullet(_bulletToSpawn, transform)
            .ContinueWith(bullet => 
            {
                if (bullet != null)     
                {
                    Vector3 randomSide = Random.value > 0.5f ? transform.right : -transform.right;
                    
                    bullet.SetDirection(randomSide); 
                    bullet.SetSpeed(_spawnedSpeed);
                    bullet.SetLifeTime(_spawnedLifeTime);
                    bullet.SetDamage(_spawnedDamage);
                    bullet.SetOwner(_owner); 
                }
            }).Forget();
    }
}
