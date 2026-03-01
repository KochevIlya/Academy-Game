

using _Project.Scripts.Scenes.Game.Infrastructure.Factory;
using _Project.Scripts.Scenes.Game.Shoot;
using _Project.Scripts.Scenes.Game.Unit;
using _Project.Scripts.Scenes.Game.Unit.Behaviour.Controls;
using _Project.Scripts.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;


public class ShotgunWeapon : RiffleWeapon
{
    [SerializeField] private float angle = 15f;
    [SerializeField] private float bulletsNum = 3;
    public override void Shoot(Vector2 shootMousePosition, GameUnit unit)
    {
        if (_currentTime >= WeaponData.CoolDown)
        {
            float fireHeight = SpawnPoint.position.y;
            _inputHelper.ScreenToGroundPosition(shootMousePosition, fireHeight, out var worldPosition); 
            var direction = (worldPosition - SpawnPoint.position).normalized;
            for (int i = 0; i < bulletsNum; i++)
            {
                
                float currentAngleOffset = 0f;
                if (bulletsNum > 1)
                {
                    currentAngleOffset = Mathf.Lerp(-angle / 2f, angle / 2f, (float)i / (bulletsNum - 1));
                }

                Vector3 finalDirection = Quaternion.Euler(0, currentAngleOffset, 0) * direction;

                SpawnAndSetup(
                    finalDirection, 
                    WeaponData.Speed, 
                    WeaponData.BulletLifeTime, 
                    WeaponData.Damage, 
                    unit
                ).Forget();
            }

            _currentTime = 0f;
        }
    }
    
}

