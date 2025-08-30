using System;
using Module.Utils.Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Managers.Router.Config.Loading
{
    [Serializable]
    public class LoadingScreenData : ScriptableEntity
    {
        [SerializeField] private AssetReference asset;

        public AddressableGameObject Asset => new(asset);
    }
}