using System;
using Module.Utils.Configs;

namespace UnityEngine.AddressableAssets
{
    [Serializable]
    public class AddressableConfig : BaseAddressableAsset<BaseConfig>
    {
        public AddressableConfig(AssetReference asset) : base(asset)
        {
        }
        
        public override bool AssetExist()
        {
            return asset != null;
        }
    }
}