namespace UnityEngine.AddressableAssets
{
    public abstract class BaseAddressableSprite : BaseAddressableAsset<Sprite>
    {
        protected BaseAddressableSprite(AssetReference asset) : base(asset)
        {
        }
        
        public override bool AssetExist()
        {
            return asset != null;
        }
    }
}