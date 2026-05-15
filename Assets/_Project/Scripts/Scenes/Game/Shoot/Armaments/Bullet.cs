using System;
using _Project.Scripts.Scenes.Game.Shoot.Config;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;
using _Project.Scripts.Scenes.Game.Unit.Components.Health;
using _Project.Sounds;
using UnityEngine.Pool;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Shoot
{
  public class Bullet : Armament
  {
    protected Vector3 _direction = Vector3.forward;
    protected float _speed = 1f;
    protected float _lifeTime = 1f;
    protected float _currentLifeTime = 0f;
    protected int _damage = 20; 
    protected GameUnit _owner;
    [Inject] protected ISoundService _soundService;
    private bool _isRemoved = false;
    

    public void SetDirection(Vector3 direction) => _direction = direction;
    public void SetSpeed(float speed) => _speed = speed;
    public void SetLifeTime(float lifeTime) => _lifeTime = lifeTime;
    public void SetDamage(int damage) => _damage = damage;
    public void SetOwner(GameUnit owner) => _owner = owner;

    protected void Update()
    {
      transform.Translate(_direction * (_speed * Time.deltaTime), Space.World);
      _currentLifeTime += Time.deltaTime;
      if (_currentLifeTime >= _lifeTime)
      {
        ResetAndRemove();
      } 
    }

    protected void OnCollisionEnter(Collision other)
    {
      Debug.Log("Collision!!");
      ResetAndRemove();
    }

    protected void OnTriggerEnter(Collider other)
    {
      if (!other.CompareTag("Bot")) return;
      if (_owner != null && other.gameObject == _owner.gameObject)
      {
        return;
      }
      if (other.TryGetComponent<Health>(out var health))
      {
        if (other.TryGetComponent<GameUnit>(out var unit))
        {
          if(unit.IsUnderControl)
            _soundService.Play(Audio.AudioType.Damage);
        }
        health.TakeDamage(_damage);
        ResetAndRemove();
      }
    }

    public override void OnSpawned()
    {
      base.OnSpawned();
      _currentLifeTime = 0f;
      _isRemoved = false;
    }
    
    protected void ResetAndRemove()
    {
      if (_isRemoved) return;
      _isRemoved = true;
      
      _currentLifeTime = 0f;
      Remove();
    } 
  }
}