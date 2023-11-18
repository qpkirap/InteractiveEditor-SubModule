using System;
using Module.InteractiveEditor.Configs;

namespace UnityEngine.AddressableAssets
{
    [Serializable]
    public class AddressableConfig : BaseAddressableAsset<BaseConfig>
    {
        public AddressableConfig()
        {
        }

        public AddressableConfig(AssetReference asset) : base(asset)
        {
        }
    }
}