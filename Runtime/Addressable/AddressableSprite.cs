using System;

namespace UnityEngine.AddressableAssets
{
    [Serializable]
    public sealed class AddressableSprite : BaseAddressableSprite
    {
        public AddressableSprite()
        {
        }

        public AddressableSprite(AssetReference asset) : base(asset)
        {
        }
    }
}