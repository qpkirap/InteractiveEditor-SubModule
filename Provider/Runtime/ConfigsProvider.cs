using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DepedencyInjection;
using Module.Utils.Configs;
using Provider.Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Provider.Runtime
{
    public class ConfigsProvider : IDisposable, IConfigsProvider
    {
        private readonly ConfigsProviderConfig config;
        
        private readonly List<AddressableConfig> assets = new();
        private readonly Dictionary<Type, BaseConfig> configs = new();
        
        public ConfigsProvider()
        {
            DI.Add<IConfigsProvider>(this);
            
            // config
            config = DI.Get<ConfigsProviderConfig>();
        }
        
        public async UniTask Init()
        {
            foreach (var asset in config.ConfigAssets)
            {
                configs.Add(asset.GetType(), asset);
            }
        }
        
        public TConfig GetConfig<TConfig>() where TConfig : BaseConfig
        {
            var configType = typeof(TConfig);

            if (configs.TryGetValue(configType, out var config))
            {
                return config as TConfig;
            }
            else
            {
                Debug.Log($"Config {configType.Name} not find!");

                return null;
            }
        }
        
        #region IDisposable
        void IDisposable.Dispose()
        {
            foreach (var asset in assets)
            {
                asset.Dispose();
            }
        }
        #endregion IDisposable
    }
}