namespace _Project.Scripts.Libs.Pool
{
  public interface IObjectPool
  {
    int ItemCount { get; }

    void Resize(int itemCount);

    void Despawn(object item);
    void Clear();
  }

  public interface IObjectPool<out T>
  {
    T Spawn();
  }

  public interface IObjectPool<out T, in TParam>
  {
    T Spawn(TParam param);
  }
}