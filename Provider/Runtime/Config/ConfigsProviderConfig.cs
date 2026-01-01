using System.Collections.Generic;
using System.Linq;
using Module.Utils.Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Provider.Configs
{
    [CreateAssetMenu]
    public class ConfigsProviderConfig : BaseConfig
    {
        [SerializeField] private List<BaseConfig> configAssets = new();

        public IEnumerable<BaseConfig> ConfigAssets => configAssets;
    }
}