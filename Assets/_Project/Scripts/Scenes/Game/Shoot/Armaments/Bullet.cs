using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Shoot
{
  public class Bullet : Armament
  {
    private Vector3 _direction = Vector3.forward;
    private float _speed = 1f;

    public void SetDirection(Vector3 direction) => _direction = direction;
    public void SetSpeed(float speed) => _speed = speed;

    private void Update()
    {
      transform.Translate(_direction * (_speed * Time.deltaTime), Space.World);
    }

    private void OnCollisionEnter(Collision other)
    {
      Debug.Log("Collision!!");
      Remove();
    }
  }
}