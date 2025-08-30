#if UNITY_EDITOR

using System.Collections.Generic;
using Managers.Router.Config;
using Managers.Router.Config.Loading;
using Module.Utils.Configs;
using UnityEditor;

namespace Managers.Router.Config
{
    /// <summary>
    /// Editor helper class containing all editor-specific methods for RouterConfig
    /// </summary>
    public static class RouterConfigEditorHelper
    {
        // Scene Data Methods
        public static void CreateNewSceneData(RouterConfig config)
        {
            UnityEngine.Debug.Log("[RouterConfigEditorHelper] Creating new scene data...");
            var instance = ScriptableEntity.Create<SceneData>();
            config.Scenes ??= new List<SceneData>();
            config.Scenes.Add(instance);
            AssetDatabase.AddObjectToAsset(instance, config);
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            
            UnityEngine.Debug.Log($"[RouterConfigEditorHelper] Scene data created. Total scenes: {config.Scenes.Count}");
            // Update script files via script generator
            RouterConfigScriptGenerator.UpdateSceneScriptFiles(config);
        }
        
        public static void RemoveSceneData(RouterConfig config, SceneData element)
        {
            if (config.Scenes != null && element != null)
            {
                config.Scenes.Remove(element);
                Undo.DestroyObjectImmediate(element);
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
                
                // Update script files via script generator
                RouterConfigScriptGenerator.UpdateSceneScriptFiles(config);
            }
        }

        // Rout Data Methods
        public static void CreateNewRoutData(RouterConfig config)
        {
            var instance = ScriptableEntity.Create<RoutData>();
            config.Routs ??= new List<RoutData>();
            config.Routs.Add(instance);
            AssetDatabase.AddObjectToAsset(instance, config);
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            
            // Update script files via script generator
            RouterConfigScriptGenerator.UpdateRoutScriptFiles(config);
        }
        
        public static void RemoveRoutData(RouterConfig config, RoutData element)
        {
            if (config.Routs != null && element != null)
            {
                config.Routs.Remove(element);
                Undo.DestroyObjectImmediate(element);
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
                
                // Update script files via script generator
                RouterConfigScriptGenerator.UpdateRoutScriptFiles(config);
            }
        }

        // Loading Data Methods
        public static void CreateNewLoadingData(RouterConfig config)
        {
            var instance = ScriptableEntity.Create<LoadingScreenData>();
            config.Loadings ??= new List<LoadingScreenData>();
            config.Loadings.Add(instance);
            AssetDatabase.AddObjectToAsset(instance, config);
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            
            // Update script files via script generator
            RouterConfigScriptGenerator.UpdateLoadingScriptFiles(config);
        }
        
        public static void RemoveLoadingData(RouterConfig config, LoadingScreenData element)
        {
            if (config.Loadings != null && element != null)
            {
                config.Loadings.Remove(element);
                Undo.DestroyObjectImmediate(element);
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
                
                // Update script files via script generator
                RouterConfigScriptGenerator.UpdateLoadingScriptFiles(config);
            }
        }
        
        // Manual script update methods
        public static void UpdateSceneKeys(RouterConfig config)
        {
            RouterConfigScriptGenerator.UpdateSceneScriptFiles(config);
            UnityEngine.Debug.Log("[RouterConfig] Scene keys updated manually");
        }
        
        public static void UpdateRoutKeys(RouterConfig config)
        {
            RouterConfigScriptGenerator.UpdateRoutScriptFiles(config);
            UnityEngine.Debug.Log("[RouterConfig] Route keys updated manually");
        }
        
        public static void UpdateLoadingKeys(RouterConfig config)
        {
            RouterConfigScriptGenerator.UpdateLoadingScriptFiles(config);
            UnityEngine.Debug.Log("[RouterConfig] Loading keys updated manually");
        }
        
        public static void UpdateAllKeys(RouterConfig config)
        {
            RouterConfigScriptGenerator.UpdateSceneScriptFiles(config);
            RouterConfigScriptGenerator.UpdateRoutScriptFiles(config);
            RouterConfigScriptGenerator.UpdateLoadingScriptFiles(config);
            UnityEngine.Debug.Log("[RouterConfig] All keys updated manually");
        }
    }
}

#endif