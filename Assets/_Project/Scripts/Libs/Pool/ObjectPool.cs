using System;
using System.Collections.Generic;

namespace _Project.Scripts.Libs.Pool
{
  public class ObjectPool<T> : IObjectPool, IObjectPool<T> where T : class
  {
    private readonly Func<T> _spawner;
    public const int DefaultInitSize = 10;

    private readonly Stack<T> _items = new Stack<T>();

    public int ItemCount => _items.Count;

    public ObjectPool()
    {
      Resize(DefaultInitSize);
    }

    public ObjectPool(Func<T> spawner)
    {
      _spawner = spawner;
      Resize(DefaultInitSize);
    }

    public ObjectPool(Func<T> spawner, int initSize)
    {
      _spawner = spawner;
      Resize(initSize);
    }

    public void Resize(int itemCount)
    {
      var countDifference = itemCount - ItemCount;
      AddElements(countDifference);
      RemoveElements(-countDifference);
    }

    private void AddElements(int count)
    {
      for (var i = 0; i < count; i++)
      {
        _items.Push(CreatePoolItem());
      }
    }

    private void RemoveElements(int count)
    {
      for (var i = 0; i < count; i++)
      {
        var item = _items.Pop();
        OnRemoveItem(item);
      }
    }

    protected virtual void OnRemoveItem(T item)
    {
    }

    public void Despawn(object item)
    {
      if (item is T typeItem)
      {
        _items.Push(typeItem);
        OnDespawnItem(typeItem);
      }
    }

    protected virtual void OnDespawnItem(T item)
    {
    }

    public virtual void Clear()
    {
      _items.Clear();
    }

    public T Spawn()
    {
      var item = _items.Count > 0 ? _items.Pop() : CreatePoolItem();
      OnItemSpawned(item);
      return item;
    }

    protected virtual void OnItemSpawned(T item)
    {
    }

    private T CreatePoolItem()
    {
      var item = _spawner.Invoke();
      OnItemCreated(item);
      return item;
    }

    protected virtual void OnItemCreated(T item)
    {
    }
  }
}