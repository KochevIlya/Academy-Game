using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Shoot;
using _Project.Scripts.Scenes.Game.Unit;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class QueuegunWeapon : RiffleWeapon
{
    
    
    [SerializeField] int bulletCount = 3;
    [SerializeField] float timeBetweenBullets = 0.05f;
    
    private bool _isShooting = false;
    public override void Shoot(Vector2 shootMousePosition, GameUnit unit)
    {
        if (_isShooting || _currentTime < WeaponData.CoolDown)
        {
            return;
        }

        ShootBurst(shootMousePosition, unit).Forget();
    }
    
    private async UniTaskVoid ShootBurst(Vector2 shootMousePosition, GameUnit unit)
    {
        _isShooting = true;

        float fireHeight = SpawnPoint.position.y;
        _inputHelper.ScreenToGroundPosition(shootMousePosition, fireHeight, out var worldPosition);
        var direction = (worldPosition - SpawnPoint.position).normalized;

        for (int i = 0; i < bulletCount; i++)
        {
            SpawnAndSetup(direction, WeaponData.Speed, WeaponData.BulletLifeTime, WeaponData.Damage, unit).Forget();

            await UniTask.Delay(System.TimeSpan.FromSeconds(timeBetweenBullets));
        }

        _isShooting = false;
        
        _currentTime = 0f;
    }
}
