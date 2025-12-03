using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Project.Scripts.Infrastructure.AssetProvider
{
    public sealed class AssetProvider : IAssetProvider
    {
        private readonly IDictionary<string, AsyncOperationHandle> _cashedHandles = 
            new Dictionary<string, AsyncOperationHandle>();

        private readonly IDictionary<string, IList<AsyncOperationHandle>> _allHandles =
            new Dictionary<string, IList<AsyncOperationHandle>>();


        T IAssetProvider.LoadFromResources<T>(string path) => Resources.Load<T>(path);
        
        T[] IAssetProvider.LoadAllFromResources<T>(string path) => Resources.LoadAll<T>(path);

        async UniTask<T> IAssetProvider.LoadFromAddressable<T>(AssetReference assetReference) where T : class 
            => await Load(Addressables.LoadAssetAsync<T>(assetReference), assetReference.AssetGUID);

        async UniTask<T> IAssetProvider.LoadFromAddressable<T>(string address) where T : class 
            => await Load(Addressables.LoadAssetAsync<T>(address), address);

        async UniTaskVoid IAssetProvider.Cleanup()
        {
            ReleaseHandles();

            await Resources.UnloadUnusedAssets();
        }

        private async UniTask<T> Load<T>(AsyncOperationHandle<T> handle, string key) where T : class
        {
            AddHandle(handle, key);

            if (_cashedHandles.TryGetValue(key, out AsyncOperationHandle cashedHandle))
            {
                return cashedHandle.Result as T;
            }
            
            handle.Completed += OnHandleCompleted<T>(key);

            return await handle.ToUniTask();
        }

        private void AddHandle<T>(AsyncOperationHandle<T> handle, string key) where T : class
        {
            if (!_allHandles.TryGetValue(key, out IList<AsyncOperationHandle> handles))
            {
                handles = new List<AsyncOperationHandle>();

                _allHandles[key] = handles;
            }

            handles.Add(handle);
        }

        private Action<AsyncOperationHandle<T>> OnHandleCompleted<T>(string address) where T : class => 
            handle => _cashedHandles[address] = handle;

        private void ReleaseHandles()
        {
            foreach (IList<AsyncOperationHandle> handles in _allHandles.Values)
            foreach (AsyncOperationHandle handle in handles)
            {
                Addressables.Release(handle);
            }
            
            _cashedHandles.Clear();
            _allHandles.Clear();
        }
    }
}