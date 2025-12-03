using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Infrastructure.AssetProvider
{
    public interface IAssetProvider
    {
        T LoadFromResources<T>(string path) where T : Object;
        T[] LoadAllFromResources<T>(string path) where T : Object;
        UniTask<T> LoadFromAddressable<T>(AssetReference assetReference) where T : class;
        UniTask<T> LoadFromAddressable<T>(string address) where T : class;
        UniTaskVoid Cleanup();
    }
}