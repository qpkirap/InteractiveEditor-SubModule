using System;
using System.Collections.Generic;
using Managers.Router.Config.Loading;
using Module.Utils.Configs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers.Router.Config
{
    /// <summary>
    /// Конфиг для хранения сцен, экранов и загрузочных экранов.
    /// Используется для создания любых конфигов роутера (модульных, игровых и т.д.)
    /// </summary>
    [CreateAssetMenu(menuName = "Router/Router Config", fileName = "RouterConfig")]
    [Serializable]
    public class BaseRouterConfig : BaseConfig, IRouterConfig
    {
        [ShowInInspector, ListDrawerSettings(
            ShowIndexLabels = false,
            ListElementLabelName = "@Title",
            DraggableItems = true,
            CustomAddFunction = "@Managers.Router.Config.BaseRouterConfigEditorHelper.CreateNewSceneData(this)",
            CustomRemoveElementFunction = "@Managers.Router.Config.BaseRouterConfigEditorHelper.RemoveSceneData(this, $removeElement)"
        )]
        [FoldoutGroup("Scenes")]
        [SerializeField] private List<SceneData> scenes;

        [ShowInInspector, ListDrawerSettings(
            ShowIndexLabels = false,
            ListElementLabelName = "@Title",
            DraggableItems = true,
            CustomAddFunction = "@Managers.Router.Config.BaseRouterConfigEditorHelper.CreateNewRoutData(this)",
            CustomRemoveElementFunction = "@Managers.Router.Config.BaseRouterConfigEditorHelper.RemoveRoutData(this, $removeElement)"
        )]
        [FoldoutGroup("Screens")]
        [SerializeField] private List<RoutData> routs;

        [ShowInInspector, ListDrawerSettings(
            ShowIndexLabels = false,
            ListElementLabelName = "@Title",
            DraggableItems = true,
            CustomAddFunction = "@Managers.Router.Config.BaseRouterConfigEditorHelper.CreateNewLoadingData(this)",
            CustomRemoveElementFunction = "@Managers.Router.Config.BaseRouterConfigEditorHelper.RemoveLoadingData(this, $removeElement)"
        )]
        [FoldoutGroup("Loading Screens")]
        [SerializeField] private List<LoadingScreenData> loadings;

        #region Script Paths

        [FoldoutGroup("Script Generation")]
        [Tooltip("Prefix for generated key classes (e.g. 'Game' -> GameRoutKeys, GameSceneKeys)")]
        [OnValueChanged("OnKeysPrefixChanged")]
        [SerializeField] private string keysPrefix;

        [FoldoutGroup("Script Generation")]
        [SerializeField] private string sceneKeysScriptPath;
        
        [FoldoutGroup("Script Generation")]
        [SerializeField] private string routsKeysScriptPath;
        
        [FoldoutGroup("Script Generation")]
        [SerializeField] private string routsArgsScriptPath;
        
        [FoldoutGroup("Script Generation")]
        [SerializeField] private string loadingsScriptPath;

        [FoldoutGroup("Script Generation")]
        [Button("Update All Keys", ButtonSizes.Large)]
        private void UpdateAllKeys()
        {
#if UNITY_EDITOR
            var helperType = System.Type.GetType("Managers.Router.Config.BaseRouterConfigEditorHelper, RouterEditor");
            var method = helperType?.GetMethod("UpdateAllKeys", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            method?.Invoke(null, new object[] { this });
#endif
        }

        private void OnKeysPrefixChanged()
        {
#if UNITY_EDITOR
            var helperType = System.Type.GetType("Managers.Router.Config.BaseRouterConfigEditorHelper, RouterEditor");
            var method = helperType?.GetMethod("OnKeysPrefixChanged", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            method?.Invoke(null, new object[] { this });
#endif
        }

        #endregion

        #region Properties

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

        public string KeysPrefix
        {
            get => keysPrefix;
            set => keysPrefix = value;
        }

        #endregion
    }
}
