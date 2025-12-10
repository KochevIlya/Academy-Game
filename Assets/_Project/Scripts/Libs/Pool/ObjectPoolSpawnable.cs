using System;
using _Project.Scripts.Libs.Pool.Item;

namespace _Project.Scripts.Libs.Pool
{
  public class ObjectPoolSpawnable<T> : ObjectPool<T> where T : class, ISpawnableItem
  {
    private readonly Func<T> _spawner;

    public ObjectPoolSpawnable()
    {

    }

    public ObjectPoolSpawnable(Func<T> spawner) : base(spawner)
    {
      _spawner = spawner;
    }

    public ObjectPoolSpawnable(Func<T> spawner, int initSize) : base(spawner, initSize)
    {
      _spawner = spawner;
      Resize(initSize);
    }

    protected override void OnItemCreated(T item)
    {
      item.OnCreated(this);
    }

    protected override void OnDespawnItem(T item)
    {
      base.OnDespawnItem(item);
      item.OnDespawned();
    }

    protected override void OnItemSpawned(T item)
    {
      base.OnItemSpawned(item);
      item.OnSpawned();
    }

    protected override void OnRemoveItem(T item)
    {
      base.OnRemoveItem(item);
      item.OnRemoved();
    }
  }
}