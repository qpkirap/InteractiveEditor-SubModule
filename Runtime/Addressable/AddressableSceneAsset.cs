using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace UnityEngine.AddressableAssets
{
    [Serializable]
    public sealed class AddressableSceneAsset : BaseAddressableStruct<SceneInstance>
    {
        [SerializeField] private bool isActiveScene;
        [SerializeField] private LoadSceneMode sceneMode;

        public bool IsActiveScene => isActiveScene;
        public LoadSceneMode SceneMode => sceneMode;

        public AddressableSceneAsset()
        {
        }

        public AddressableSceneAsset(AssetReference asset) : base(asset)
        {
        }

        public void SetLoadSceneMode(LoadSceneMode sceneMode)
        {
            this.sceneMode = sceneMode;
        }

        protected override async UniTask<SceneInstance> DoLoad(CancellationToken token = default)
        {
            handle = Addressables.LoadSceneAsync(assetReference, sceneMode);
            asset = await handle.WithCancellation(token);
            return asset;
        }

        protected override async UniTask DoRelease()
        {
            await Addressables.UnloadSceneAsync(handle);
            handle = default;
            asset = default;
        }
    }
}