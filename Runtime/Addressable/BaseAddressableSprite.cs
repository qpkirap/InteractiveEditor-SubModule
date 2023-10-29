namespace UnityEngine.AddressableAssets
{
    public abstract class BaseAddressableSprite : BaseAddressableAsset<Sprite>
    {
        protected BaseAddressableSprite()
        {
        }

        protected BaseAddressableSprite(AssetReference asset) : base(asset)
        {
        }
    }
}