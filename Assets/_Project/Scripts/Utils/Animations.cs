using UnityEngine;

namespace _Project.Scripts.Utils
{
  public static class Animations
  {
    public static int Velocity => Animator.StringToHash(nameof(Velocity));
    public static int Shoot => Animator.StringToHash(nameof(Shoot));
  }
}