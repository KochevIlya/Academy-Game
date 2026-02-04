using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using _Project.Scripts.Scenes.Game.Unit.Controls.Variants;

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
    
    public override void Shoot(Vector2 shootMousePosition, GameUnit unit)
    {
      if (_currentTime < WeaponData.CoolDown) return;

      Vector3 targetWorldPosition;

      if (unit.InputControls is UserInputControls)
      {
        _inputHelper.ScreenToGroundPosition(shootMousePosition, unit.transform.position.y, out targetWorldPosition);
      }
      else
      {
        targetWorldPosition = new Vector3(shootMousePosition.x, unit.transform.position.y, shootMousePosition.y);
      }
      Vector3 spawnPos = SpawnPoint.position;
      Vector3 direction = (targetWorldPosition - spawnPos);
      direction.y = 0;
      direction = direction.normalized;

      if (direction.sqrMagnitude > 0.001f)
      {
        SpawnAndSetup(direction, WeaponData.Speed, WeaponData.BulletLifeTime, WeaponData.Damage, unit).Forget();
        _currentTime = 0f;
      }
    }
    
    private async UniTaskVoid SpawnAndSetup(Vector3 direction, float speed, float lifetime, int damage, GameUnit unit)
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