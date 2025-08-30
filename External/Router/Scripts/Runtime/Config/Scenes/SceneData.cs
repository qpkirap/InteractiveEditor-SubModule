using System;
using Module.Utils.Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Managers.Router.Config
{
    [Serializable]
    public class SceneData : ScriptableEntity
    {
        [SerializeField] private AssetReference sceneAsset;
        
        public AddressableSceneAsset Asset => new(sceneAsset);
    }
}