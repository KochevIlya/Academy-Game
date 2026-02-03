using System;
using _Project.Scripts.Scenes.Game.Shoot.Config;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;
using _Project.Scripts.Scenes.Game.Unit.Components.Health;
using UnityEngine.Pool;

namespace _Project.Scripts.Scenes.Game.Shoot
{
  public class Bullet : Armament
  {
    private Vector3 _direction = Vector3.forward;
    private float _speed = 1f;
    private float _lifeTime = 1f;
    private float _currentLifeTime = 0f;
    private int _damage = 20; 
    private GameUnit _owner;
    

    public void SetDirection(Vector3 direction) => _direction = direction;
    public void SetSpeed(float speed) => _speed = speed;
    public void SetLifeTime(float lifeTime) => _lifeTime = lifeTime;
    public void SetDamage(int damage) => _damage = damage;
    public void SetOwner(GameUnit owner) => _owner = owner;

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
      if (other.gameObject.tag == "Bot" && other.gameObject != _owner.gameObject)
      {
        other.gameObject.GetComponent<Health>().TakeDamage(_damage);
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