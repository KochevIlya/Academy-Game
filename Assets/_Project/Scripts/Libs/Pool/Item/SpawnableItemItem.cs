namespace _Project.Scripts.Libs.Pool.Item
{
  public class SpawnableItemItem : ISpawnableItem
  {
    private IObjectPool _objectPool;

    public virtual void OnCreated(IObjectPool objectPool)
    {
      _objectPool = objectPool;
    }

    public virtual void OnSpawned() { }
    public virtual void OnDespawned() { }
    public virtual void OnRemoved() { }

    public void Remove()
    {
      if (_objectPool is null)
      {
        OnRemoved();
        return;
      }

      _objectPool.Despawn(this);
    }
  }
}