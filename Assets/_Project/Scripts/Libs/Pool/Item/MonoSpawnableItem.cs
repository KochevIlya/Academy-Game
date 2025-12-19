using UnityEngine;

namespace _Project.Scripts.Libs.Pool.Item
{
  public abstract class MonoSpawnableItem : MonoBehaviour, ISpawnableItem
  {
    private IObjectPool _objectPool;

    public virtual void OnCreated(IObjectPool objectPool)
    {
      _objectPool = objectPool;
    }

    public virtual void OnSpawned() { }
    public virtual void OnDespawned() { }
    public virtual void OnRemoved()
    {
      Destroy(gameObject);
    }

    public virtual void Remove()
    {
      if (_objectPool is null)
      {
        Debug.Log("The bullet did not return to pool");
        OnRemoved();
        return;
      }
      gameObject.SetActive(false);
      Debug.Log("The bullet returned to pool");
      _objectPool.Despawn(this);
    }
  }
}