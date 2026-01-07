#if UNITY_EDITOR

using System.Collections.Generic;
using Managers.Router.Config;
using Module.Utils.Configs;
using UnityEditor;

namespace Managers.Router.Config
{
    /// <summary>
    /// Editor helper class containing all editor-specific methods for RoutData
    /// </summary>
    public static class RoutDataEditorHelper
    {
        // Rout Arg Data Methods
        public static void CreateNewArgData(RoutData routData)
        {
            var instance = ScriptableEntity.Create<RoutArgData>();
            routData.ArgsData ??= new List<RoutArgData>();
            routData.ArgsData.Add(instance);
            AssetDatabase.AddObjectToAsset(instance, routData);
            EditorUtility.SetDirty(routData);
            AssetDatabase.SaveAssets();
            
            // Update parent RouterConfig script files
            UpdateParentRouterConfigScriptFiles(routData);
        }
        
        public static void RemoveArgData(RoutData routData, RoutArgData element)
        {
            if (routData.ArgsData != null && element != null)
            {
                routData.ArgsData.Remove(element);
                Undo.DestroyObjectImmediate(element);
                EditorUtility.SetDirty(routData);
                AssetDatabase.SaveAssets();
                
                // Update parent RouterConfig script files
                UpdateParentRouterConfigScriptFiles(routData);
            }
        }
        
        private static void UpdateParentRouterConfigScriptFiles(RoutData routData)
        {
            // Find the parent BaseRouterConfig that contains this RoutData
            var routerConfigs = AssetDatabase.FindAssets($"t:{typeof(BaseRouterConfig).Name}");
            foreach (var guid in routerConfigs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var routerConfig = AssetDatabase.LoadAssetAtPath<BaseRouterConfig>(path);
                if (routerConfig != null)
                {
                    // Check if this RoutData belongs to this config
                    var routs = routerConfig.Routs;
                    if (routs != null && routs.Contains(routData))
                    {
                        // Update script files via script generator
                        RouterConfigScriptGenerator.UpdateRoutScriptFiles(routerConfig);
                        break;
                    }
                }
            }
        }
    }
}

#endif