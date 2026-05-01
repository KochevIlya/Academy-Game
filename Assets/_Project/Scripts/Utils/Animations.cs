using UnityEngine;

namespace _Project.Scripts.Utils
{
  public static class Animations
  {
    public static int VelocityX => Animator.StringToHash(nameof(VelocityX));
    public static int VelocityY => Animator.StringToHash(nameof(VelocityY));
    public static int Shoot => Animator.StringToHash(nameof(Shoot));
    public static int PushingButton => Animator.StringToHash(nameof(PushingButton));
    public static int IsRunning => Animator.StringToHash(nameof(IsRunning));
    public static int Die => Animator.StringToHash(nameof(Die));
  }
}