namespace _Project.Scripts.Libs.Pool.Item
{
  public interface ISpawnableItem
  {
    void OnCreated(IObjectPool objectPool);
    void OnSpawned();
    void OnDespawned();
    void OnRemoved();
  }
}