namespace UnityEngine.AddressableAssets
{
    public abstract class BaseAddressableStruct<TAsset> : BaseAddressableAsset<TAsset>
        where TAsset : struct
    {
        protected BaseAddressableStruct()
        {
        }

        protected BaseAddressableStruct(AssetReference asset) : base(asset)
        {
        }

        internal override bool AssetExist()
        {
            return !Equals(asset, default(TAsset));
        }
    }
}