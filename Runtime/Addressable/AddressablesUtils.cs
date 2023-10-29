using Cysharp.Threading.Tasks;

namespace UnityEngine.AddressableAssets
{
    public static class AddressablesUtils
    {
        public static async UniTask<bool> AssetExists(object key)
        {
            var handle = Addressables.LoadResourceLocationsAsync(key);

            var locations = await handle.Task;
            var exists = locations.Count > 0;

            Addressables.Release(handle);

            return exists;
        }

        public static void CreateOrUpdateAsset<T>(ref T currentAsset, T newAsset)
            where T : IAddressableAsset
        {
            var assetExist = currentAsset != null;

            if (assetExist && currentAsset.AssetGUID == newAsset.AssetGUID)
            {
                return;
            }

            if (assetExist)
            {
                currentAsset.Dispose();
            }

            currentAsset = newAsset;
        }
    }
}