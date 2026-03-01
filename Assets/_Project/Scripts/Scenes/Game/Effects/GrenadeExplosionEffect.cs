using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GrenadeExplosionEffect : MonoBehaviour
{
    public void Initialize(float radius, float duration)
    {
        transform.localScale = Vector3.one * radius * 2f;
        
        DestroyAfterDelay(duration).Forget();
    }

    private async UniTaskVoid DestroyAfterDelay(float duration)
    {
        await UniTask.Delay((int)(duration * 1000));
        
        Destroy(gameObject);
    }
}
