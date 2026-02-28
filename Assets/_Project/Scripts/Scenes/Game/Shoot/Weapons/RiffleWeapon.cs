using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Shoot
{
  public class RiffleWeapon : WeaponBase
  {
    protected IGameFactory _gameFactory;
    protected IInputHelper _inputHelper;
    protected float _currentTime; 

    [Inject]
    public void Construct(IGameFactory gameFactory, IInputHelper inputHelper)
    {
      _inputHelper = inputHelper;
      _gameFactory = gameFactory;
    }

    protected void Update()
    {
      if (_currentTime < WeaponData.CoolDown)
      {
        _currentTime += Time.deltaTime;
      }
    }
    
    public override void Shoot(Vector2 shootMousePosition, GameUnit unit)
    {
      if (_currentTime >= WeaponData.CoolDown)
      {
        float fireHeight = SpawnPoint.position.y;
        _inputHelper.ScreenToGroundPosition(shootMousePosition, fireHeight, out var worldPosition); 
        var direction = (worldPosition - SpawnPoint.position).normalized;
        SpawnAndSetup(direction, WeaponData.Speed, WeaponData.BulletLifeTime, WeaponData.Damage, unit).Forget();
        _currentTime = 0f;
      }
    }
    
    protected async UniTaskVoid SpawnAndSetup(Vector3 direction, float speed, float lifetime, int damage, GameUnit unit)
    {
      Bullet bullet = await _gameFactory.SpawnBullet(WeaponData.Bullet, SpawnPoint);
      bullet.SetDirection(direction);
      bullet.SetSpeed(speed);
      bullet.SetLifeTime(lifetime);
      bullet.SetDamage(damage);
      bullet.SetOwner(unit);
    }
  }
}