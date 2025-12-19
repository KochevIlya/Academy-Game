using _Project.Scripts.Scenes.Game.Shoot.Config;
using UnityEngine;
using UnityEngine.Pool;

namespace _Project.Scripts.Scenes.Game.Shoot
{
  public class Bullet : Armament
  {
    private Vector3 _direction = Vector3.forward;
    private float _speed = 1f;
    private float _lifeTime = 0f;
    

    public void SetDirection(Vector3 direction) => _direction = direction;
    public void SetSpeed(float speed) => _speed = speed;

    private void Update()
    {
      transform.Translate(_direction * (_speed * Time.deltaTime), Space.World);
      _lifeTime += Time.deltaTime;
      if (_lifeTime >= WeaponsConfig.BulletLifeTime)
      {
        _lifeTime = 0f;
        Remove();
      }
    }

    private void OnCollisionEnter(Collision other)
    {
      Debug.Log("Collision!!");
      _lifeTime = 0f;
      Remove();
    }
  }
}