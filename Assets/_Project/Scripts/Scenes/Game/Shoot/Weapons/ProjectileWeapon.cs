using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Scenes.Game.Shoot
{
  public class ProjectileWeapon : WeaponBase
  {
    private IGameFactory _gameFactory;
    private IInputHelper _inputHelper;
    private float _currentTime;

    [Inject]
    public void Construct(IGameFactory gameFactory, IInputHelper inputHelper)
    {
      _inputHelper = inputHelper;
      _gameFactory = gameFactory;
    }

    private void Update()
    {
      if (_currentTime < WeaponData.CoolDown)
      {
        _currentTime += Time.deltaTime;
      }
    }
    
    public override void Shoot(Vector2 shootMousePosition)
    {
      if (_currentTime >= WeaponData.CoolDown)
      {
        _inputHelper.ScreenToGroundPosition(shootMousePosition, Unit.transform.position.y, out var worldPosition); 
        var direction = (worldPosition - Unit.transform.position).SetY(0f).normalized;
        SpawnAndSetup(direction, WeaponData.Speed, WeaponData.BulletLifeTime).Forget();
        _currentTime = 0f;
      }
    }
    
    private async UniTaskVoid SpawnAndSetup(Vector3 direction, float speed, float lifetime)
    {
      Bullet bullet = await _gameFactory.SpawnBullet(WeaponData.Bullet, SpawnPoint);
      bullet.SetDirection(direction);
      bullet.SetSpeed(speed);
      bullet.SetLifeTime(lifetime);
    }
  }
}