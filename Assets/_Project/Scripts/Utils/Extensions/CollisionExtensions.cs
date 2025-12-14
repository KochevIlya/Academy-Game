namespace _Project.Scripts.Utils.Extensions
{
  public static class CollisionExtensions
  {
    public static bool Matches(this int mask, CollisionLayer layer) =>
      (mask & layer.AsMask()) != 0;

    public static int AsMask(this CollisionLayer layer) =>
      1 << (int)layer;
  }
}