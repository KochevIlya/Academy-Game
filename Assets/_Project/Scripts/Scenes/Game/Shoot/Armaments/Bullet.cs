using UnityEngine;
using UnityEngine.Pool;

namespace _Project.Scripts.Scenes.Game.Shoot
{
  public class Bullet : Armament
  {
    private Vector3 _direction = Vector3.forward;
    private float _speed = 1f;
    private float _timeToLive = 3f;
    private float _lifeTime = 0f;
    
    private IObjectPool<Bullet> _pool;

    public void SetDirection(Vector3 direction) => _direction = direction;
    public void SetSpeed(float speed) => _speed = speed;
    
    public void SetPool(IObjectPool<Bullet> pool) => _pool = pool;

    private void Update()
    {
      transform.Translate(_direction * (_speed * Time.deltaTime), Space.World);
      _lifeTime += Time.deltaTime;
      if (_lifeTime >= _timeToLive)
      {
        Release();
      }
    }

    private void OnCollisionEnter(Collision other)
    {
      Debug.Log("Collision!!");
      Release();
    }
    
    private void Release()
    {
      _lifeTime = 0f;
      _pool.Release(this);
    }
  }
}