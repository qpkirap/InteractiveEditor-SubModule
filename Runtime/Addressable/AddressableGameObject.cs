#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace UnityEngine.AddressableAssets
{
    [Serializable]
    public sealed class AddressableGameObject : BaseAddressableAsset<GameObject>
    {
        private InstantiationParameters instantiateParameters;

        public AddressableGameObject()
        {
        }

        public AddressableGameObject(AssetReference asset) : base(asset)
        {
        }

        public void SetParent(GameObject parent)
        {
            SetParent(parent.transform);
        }

        public void SetParent(Component parent)
        {
            instantiateParameters = new InstantiationParameters(parent.transform, instantiateInWorldSpace: false);
        }

        public void SetParent(Vector3 position, Quaternion rotation, GameObject parent)
        {
            var transform = parent == null ? null : parent.transform;

            SetParent(position, rotation, transform);
        }

        public void SetParent(Vector3 position, Quaternion rotation, Component parent)
        {
            var transform = parent == null ? null : parent.transform;

            SetParent(position, rotation, transform);
        }

        public void SetParent(Vector3 position, Quaternion rotation, Transform transform = null)
        {
            instantiateParameters = new InstantiationParameters(position, rotation, transform);
        }

        protected override async UniTask<GameObject> DoLoad(CancellationToken token)
        {
            handle = Addressables.InstantiateAsync(assetReference, instantiateParameters);
            asset = await handle.WithCancellation(token);
            return asset;
        }

        protected override async UniTask DoRelease()
        {
            Addressables.ReleaseInstance(handle);
            handle = default;
            asset = null;
        }
    }
}