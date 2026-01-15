using System;
using _Project.Scripts.Scenes.Game.Shoot.Config;
using UnityEngine;
using UnityEngine.Pool;

namespace _Project.Scripts.Scenes.Game.Shoot
{
  public class Bullet : Armament
  {
    private Vector3 _direction = Vector3.forward;
    private float _speed = 1f;
    private float _lifeTime = 1f;
    private float _currentLifeTime = 0f;
    

    public void SetDirection(Vector3 direction) => _direction = direction;
    public void SetSpeed(float speed) => _speed = speed;
    public void SetLifeTime(float lifeTime) => _lifeTime = lifeTime;

    private void Update()
    {
      transform.Translate(_direction * (_speed * Time.deltaTime), Space.World);
      _currentLifeTime += Time.deltaTime;
      if (_currentLifeTime >= _lifeTime)
      {
        ResetAndRemove();
      }
    }

    private void OnCollisionEnter(Collision other)
    {
      Debug.Log("Collision!!");
      ResetAndRemove();
    }

    private void OnTriggerEnter(Collider other)
    {
      if (other.gameObject.tag == "Player")
      {
        ResetAndRemove();
      }
    }
    
    private void ResetAndRemove()
    {
      _currentLifeTime = 0f;
      Remove();
    }
  }
}