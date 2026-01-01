namespace UnityEngine.AddressableAssets
{
    public abstract class BaseAddressableStruct<TAsset> : BaseAddressableAsset<TAsset>
        where TAsset : struct
    {
        protected BaseAddressableStruct(AssetReference asset) : base(asset)
        {
        }

        public override bool AssetExist()
        {
            return !Equals(asset, default(TAsset));
        }
    }
}