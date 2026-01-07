using System;
using Module.Utils.Configs;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Managers.Router.Config
{
    /// <summary>
    /// Настройки роутера: контроллеры.
    /// </summary>
    [CreateAssetMenu(menuName = "Router/Router Settings", fileName = "RouterSettings")]
    [Serializable]
    public class RouterSettings : BaseConfig
    {
        [FoldoutGroup("Controllers")]
        [SerializeField] private AssetReference loadingScreenControllerAsset;
        
        [FoldoutGroup("Controllers")]
        [SerializeField] private AssetReference routerControllerAsset;

        public AddressableGameObject LoadingScreenControllerAsset => new(loadingScreenControllerAsset);
        public AddressableGameObject RouterControllerAsset => new(routerControllerAsset);
    }
}
