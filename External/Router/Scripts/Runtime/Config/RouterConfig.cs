using System;
using System.Collections.Generic;
using System.Linq;
using Managers.Router.Config.Loading;
using Module.Utils.Configs;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Managers.Router.Config
{
    [CreateAssetMenu][Serializable]
    public class RouterConfig : BaseConfig
    {
        [ShowInInspector, ListDrawerSettings(
            ShowIndexLabels = false,
            ListElementLabelName = "@Title",
            DraggableItems = true,
            CustomAddFunction = "@Managers.Router.Config.RouterConfigEditorHelper.CreateNewSceneData(this)",
            CustomRemoveElementFunction = "@Managers.Router.Config.RouterConfigEditorHelper.RemoveSceneData(this, $removeElement)"
        )]
        [FoldoutGroup("Configuration Lists")]
        [SerializeField] private List<SceneData> scenes;
        
        [ShowInInspector, ListDrawerSettings(
            ShowIndexLabels = false,
            ListElementLabelName = "@Title",
            DraggableItems = true,
            CustomAddFunction = "@Managers.Router.Config.RouterConfigEditorHelper.CreateNewRoutData(this)",
            CustomRemoveElementFunction = "@Managers.Router.Config.RouterConfigEditorHelper.RemoveRoutData(this, $removeElement)"
        )]
        [FoldoutGroup("Configuration Lists")]
        [SerializeField] private List<RoutData> routs;
        
        [ShowInInspector, ListDrawerSettings(
            ShowIndexLabels = false,
            ListElementLabelName = "@Title",
            DraggableItems = true,
            CustomAddFunction = "@Managers.Router.Config.RouterConfigEditorHelper.CreateNewLoadingData(this)",
            CustomRemoveElementFunction = "@Managers.Router.Config.RouterConfigEditorHelper.RemoveLoadingData(this, $removeElement)"
        )]
        [FoldoutGroup("Configuration Lists")]
        [SerializeField] private List<LoadingScreenData> loadings;
        [Space]
        [SerializeField] private AssetReference loadingScreenControllerAsset;
        [SerializeField] private AssetReference routerControllerAsset;

        #region Editor

        [SerializeField] private string sceneKeysScriptPath;
        [SerializeField] private string routsKeysScriptPath;
        [SerializeField] private string routsArgsScriptPath;
        [SerializeField] private string loadingsScriptPath;
        
        public const string SceneDataKey = nameof(scenes);
        public const string RoutDataKey = nameof(routs);
        public const string LoadingDataKey = nameof(loadings);
        public const string SceneKeysScriptPathKey = nameof(sceneKeysScriptPath);
        public const string RoutKeysScriptPathKey = nameof(routsKeysScriptPath);
        public const string RoutArgsScriptPathKey = nameof(routsArgsScriptPath);
        public const string LoadingsScriptPathKey = nameof(loadingsScriptPath);

        // Public properties for editor access
        public List<SceneData> Scenes
        {
            get => scenes;
            set => scenes = value;
        }
        
        public List<RoutData> Routs
        {
            get => routs;
            set => routs = value;
        }
        
        public List<LoadingScreenData> Loadings
        {
            get => loadings;
            set => loadings = value;
        }
        
        // Public properties for script paths (editor access)
        public string SceneKeysScriptPath
        {
            get => sceneKeysScriptPath;
            set => sceneKeysScriptPath = value;
        }
        
        public string RoutKeysScriptPath
        {
            get => routsKeysScriptPath;
            set => routsKeysScriptPath = value;
        }
        
        public string RoutArgsScriptPath
        {
            get => routsArgsScriptPath;
            set => routsArgsScriptPath = value;
        }
        
        public string LoadingsScriptPath
        {
            get => loadingsScriptPath;
            set => loadingsScriptPath = value;
        }

        #endregion
        
        private static Dictionary<SceneKey, SceneData> scenesDict;
        private static Dictionary<LoadingScreenKey, LoadingScreenData> loadingScreenDict;
        private static Dictionary<RoutKey, RoutData> routsDict;
        private static Dictionary<string, int> routSortOrderDict;
        
        public AddressableGameObject LoadingScreenControllerAsset => new(loadingScreenControllerAsset);
        public AddressableGameObject RouterControllerAsset => new(routerControllerAsset);
        
        public SceneData GetScene(SceneKey key)
        {
            if (scenesDict == null)
            {
                scenesDict = scenes.ToDictionary(x => (SceneKey)x.Id, x => x);
            }

            if (scenesDict.TryGetValue(key, out var sceneData))
            {
                return sceneData;
            }
            else
            {
                Debug.Log($"Сцены с указанным ключом не существует: {key}");

                return null;
            }
        }
        
        public LoadingScreenData GetLoadingScreen(LoadingScreenKey key)
        {
            if (key == null)
            {
                return null;
            }

            if (loadingScreenDict == null)
            {
                loadingScreenDict = loadings.ToDictionary(x => (LoadingScreenKey)x.Id, x => x);
            }

            if (loadingScreenDict.TryGetValue(key, out var loadingScreenData))
            {
                return loadingScreenData;
            }
            else
            {
                Debug.Log($"Экрана загрузки с указанным ключом не существует: {key}");

                return null;
            }
        }
        
        public RoutData GetRout(RoutKey key)
        {
            if (routsDict == null)
            {
                routsDict = routs
                    .ToDictionary(x => (RoutKey)x.Id, x => x);
            }

            if (routsDict.TryGetValue(key, out var routData))
            {
                return routData;
            }
            else
            {
                Debug.Log($"Роута с указанным ключом не существует: {key}");

                return null;
            }
        }
        
        public int GetRoutSortOrder(RoutData data)
        {
            if (data == default) return 0;
            
            var dataKey = data.Id;

            routSortOrderDict ??= routs.ToDictionary(x => x.Id, x => x.SortOrder);

            return routSortOrderDict.TryGetValue(dataKey, out var sortOrder)
                ? sortOrder
                : 0;
        }
    }
}