using System;
using _Project.Scripts.Libs.Pool.Item;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Libs.Pool
{
  public class ObjectPoolMono<T> : ObjectPoolSpawnable<T> where T : Component, ISpawnableItem
  {

    private readonly Transform _poolTransform;

    public static ObjectPoolMono<T> CreatePoolMono(GameObject prefab, int count = DefaultInitSize, Transform transform = null)
    {
      return new ObjectPoolMono<T>(() => Object.Instantiate(prefab, transform).GetComponent<T>(), count, transform);
    }

    public ObjectPoolMono()
    {

    }

    public ObjectPoolMono(Func<T> spawner) : base(spawner)
    {
    }

    public ObjectPoolMono(Func<T> spawner, int initSize, Transform poolTransform) : base(spawner, initSize)
    {
      _poolTransform = poolTransform;
    }

    protected override void OnItemSpawned(T item)
    {
      base.OnItemSpawned(item);
      item.gameObject.SetActive(true);
    }

    protected override void OnDespawnItem(T item)
    {
      base.OnDespawnItem(item);
      DeactivateItem(item);
    }

    private void DeactivateItem(T item)
    {
      item.gameObject.SetActive(false);
      item.gameObject.transform.SetParent(_poolTransform);
    }

    protected override void OnItemCreated(T item)
    {
      base.OnItemCreated(item);
      DeactivateItem(item);
    }
  }
}