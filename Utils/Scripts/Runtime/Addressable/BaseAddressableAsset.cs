﻿#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.AddressableAssets
{
    public abstract class BaseAddressableAsset<TAsset> : IAddressableAsset, IDisposable, ICloneable
    {
        private const string invalidHandleName = "InvalidHandle";

        [JsonProperty, NonSerialized] protected AssetReference assetReference = new();

        [NonSerialized] protected TAsset asset;
        [NonSerialized] protected AsyncOperationHandle<TAsset> handle;
        
        [NonSerialized] private UniTaskCompletionSource<TAsset> loadCompletionSource;
        [NonSerialized] private UniTaskCompletionSource<TAsset> releaseCompletionSource;

        [JsonIgnore] public string AssetGUID => assetReference.AssetGUID;
        [JsonIgnore] public bool RuntimeKeyIsValid => assetReference.RuntimeKeyIsValid();

        public BaseAddressableAsset(AssetReference asset)
        {
            assetReference = new AssetReference(asset.AssetGUID)
            {
                SubObjectName = asset.SubObjectName
            };
        }

        public async UniTask PreloadAsync(CancellationToken token = default)
        {
            await LoadAsync(token);
        }

        public async UniTask<TAsset> LoadAsync(CancellationToken token = default)
        {
            if (releaseCompletionSource != null)
            {
                await releaseCompletionSource.Task;
            }
            
            if (loadCompletionSource != null)
            {
                return await loadCompletionSource.Task;
            }
            
            loadCompletionSource = new UniTaskCompletionSource<TAsset>();
            
            await DoLoad().ContinueWith((t) => loadCompletionSource.TrySetResult(t));

            if (loadCompletionSource == null) return await LoadAsync();
            
            return await loadCompletionSource.Task;
        }

        public virtual bool AssetExist()
        {
            return asset != null || handle.DebugName == invalidHandleName;
        }

        public async void Dispose()
        {
            await ReleaseAsync();
        }

        public async UniTask Release()
        {
            await ReleaseAsync();
        }

        public async UniTask ReleaseAsync()
        {
            if (releaseCompletionSource != null)
            {
                await releaseCompletionSource.Task;
                
                return;
            }
            
            if (loadCompletionSource != null) await loadCompletionSource.Task;
            
            releaseCompletionSource = new UniTaskCompletionSource<TAsset>();
            
            await DoRelease().ContinueWith(() => releaseCompletionSource.TrySetResult(asset));

            loadCompletionSource = null;
            releaseCompletionSource = null;
        }

        protected virtual async UniTask<TAsset> DoLoad()
        {
            handle = Addressables.LoadAssetAsync<TAsset>(assetReference);
            
            asset = await handle;
            
            return asset;
        }

        protected virtual async UniTask DoRelease()
        {
            if (handle.DebugName == invalidHandleName) return;
            
            Addressables.Release(handle);
            handle = default;
            asset = default;
        }

        public object Clone()
        {
            var asset = MemberwiseClone() as BaseAddressableAsset<TAsset>;

            asset.assetReference = new AssetReference(assetReference.AssetGUID)
            {
                SubObjectName = assetReference.SubObjectName
            };

            return asset;
        }

        public static implicit operator AssetReference(BaseAddressableAsset<TAsset> asset) => asset.assetReference;
    }
}