using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Scenes.Game.Shoot;
using _Project.Scripts.Scenes.Game.Unit;
using UnityEngine;

public class SpawnedgunWeapon : RiffleWeapon
{
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
    
    
}
