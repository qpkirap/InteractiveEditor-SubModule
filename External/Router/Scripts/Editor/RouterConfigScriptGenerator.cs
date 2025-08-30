#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Module.Utils.Editor;
using UnityEditor;
using UnityEngine;

namespace Managers.Router.Config
{
    /// <summary>
    /// Editor-only helper class for script file generation in RouterConfig
    /// </summary>
    public static class RouterConfigScriptGenerator
    {
        public static void UpdateSceneScriptFiles(RouterConfig routerConfig)
        {
            Debug.Log("[ScriptGen] Starting scene keys update...");
            var sceneKeyData = GetSceneKeyScriptData(routerConfig);
            UpdateOrCreateScriptFile(sceneKeyData);
            Debug.Log("[ScriptGen] Scene keys update completed");
        }
        
        public static void UpdateRoutScriptFiles(RouterConfig routerConfig)
        {
            var routKeyData = GetRoutKeyScriptData(routerConfig);
            var routArgsKeyData = GetRoutArgsKeyScriptData(routerConfig);
            UpdateOrCreateScriptFile(routKeyData);
            UpdateOrCreateScriptFile(routArgsKeyData);
        }
        
        public static void UpdateLoadingScriptFiles(RouterConfig routerConfig)
        {
            var loadingKeyData = GetLoadingKeyScriptData(routerConfig);
            UpdateOrCreateScriptFile(loadingKeyData);
        }
        
        /// <summary>
        /// Updates existing script or creates new one if not found
        /// </summary>
        private static void UpdateOrCreateScriptFile(KeyScriptData scriptData)
        {
            Debug.Log($"[ScriptGen] Updating/creating script: {scriptData.FileName}");
            var currentPath = scriptData.getPathValue?.Invoke();
            Debug.Log($"[ScriptGen] Current path: {currentPath ?? "null"}");
            
            // If no path is set, try to find existing script
            if (string.IsNullOrEmpty(currentPath))
            {
                Debug.Log("[ScriptGen] No path set, searching for existing script...");
                var foundPath = FindExistingScript(scriptData.FileName);
                if (!string.IsNullOrEmpty(foundPath))
                {
                    Debug.Log($"[ScriptGen] Found existing script at: {foundPath}");
                    // Update the config with found path
                    scriptData.onPathCreated?.Invoke(foundPath);
                    KeyScriptFileUtils.UpdateScriptFile(scriptData);
                    return;
                }
                
                Debug.Log("[ScriptGen] No existing script found, creating new one...");
                // No existing script found, create new one
                CreateNewScriptFile(scriptData);
                return;
            }
            
            // Check if the current path exists
            var fullPath = Path.Combine(Application.dataPath, currentPath.TrimStart('/', '\\'));
            Debug.Log($"[ScriptGen] Checking full path: {fullPath}");
            if (File.Exists(fullPath))
            {
                Debug.Log("[ScriptGen] Path exists, updating script...");
                // Path exists, update the script
                KeyScriptFileUtils.UpdateScriptFile(scriptData);
            }
            else
            {
                Debug.Log("[ScriptGen] Path doesn't exist, searching for existing script...");
                // Path doesn't exist, try to find existing script or create new one
                var foundPath = FindExistingScript(scriptData.FileName);
                if (!string.IsNullOrEmpty(foundPath))
                {
                    Debug.Log($"[ScriptGen] Found existing script at: {foundPath}, updating path");
                    scriptData.onPathCreated?.Invoke(foundPath);
                    KeyScriptFileUtils.UpdateScriptFile(scriptData);
                }
                else
                {
                    Debug.Log("[ScriptGen] No existing script found, creating new one...");
                    CreateNewScriptFile(scriptData);
                }
            }
            Debug.Log($"[ScriptGen] Script update completed for: {scriptData.FileName}");
        }
        
        /// <summary>
        /// Search for existing script file in the project
        /// </summary>
        private static string FindExistingScript(string fileName)
        {
            var guids = AssetDatabase.FindAssets($"{fileName} t:MonoScript");
            
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var scriptName = Path.GetFileNameWithoutExtension(assetPath);
                
                if (scriptName == fileName)
                {
                    // Convert to relative path from Assets folder using proper path separators
                    var relativePath = assetPath.Substring("Assets".Length);
                    // Ensure we use forward slashes for Unity asset paths
                    return relativePath.Replace('\\', '/');
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Create new script file with default location
        /// </summary>
        private static void CreateNewScriptFile(KeyScriptData scriptData)
        {
            // Create default path in Router/Scripts/Runtime/Config/Generated folder using cross-platform paths
            var defaultFolderParts = new[] { "Assets", "6 - Submodules", "InteractiveEditor", "External", "Router", "Scripts", "Runtime", "Config", "Generated" };
            var defaultFolder = Path.Combine(defaultFolderParts);
            
            // Convert to Unity asset path format (forward slashes)
            var unityAssetPath = defaultFolder.Replace('\\', '/');
            
            // Get the physical directory path
            var physicalFolderPath = Path.Combine(Application.dataPath, 
                Path.Combine(defaultFolderParts.Skip(1).ToArray())); // Skip "Assets" part
            
            // Ensure directory exists
            if (!Directory.Exists(physicalFolderPath))
            {
                UnityEngine.Debug.Log($"[ScriptGen] Creating directory: {physicalFolderPath}");
                Directory.CreateDirectory(physicalFolderPath);
            }
            
            var fileName = scriptData.FileName;
            var assetPath = Path.Combine(unityAssetPath, $"{fileName}.cs").Replace('\\', '/');
            var physicalFilePath = Path.Combine(physicalFolderPath, $"{fileName}.cs");
            
            // Check if file already exists in default location
            if (File.Exists(physicalFilePath))
            {
                // File exists, just update the path
                var relativePath = assetPath.Substring("Assets".Length).Replace('\\', '/');
                UnityEngine.Debug.Log($"[ScriptGen] File exists, updating path to: {relativePath}");
                scriptData.onPathCreated?.Invoke(relativePath);
                KeyScriptFileUtils.UpdateScriptFile(scriptData);
            }
            else
            {
                UnityEngine.Debug.Log($"[ScriptGen] Creating new file at: {assetPath}");
                // Create new file
                KeyScriptFileUtils.CreateScriptFile(assetPath, scriptData);
            }
            
            // Refresh the AssetDatabase
            AssetDatabase.Refresh();
        }
        
        // Key script data providers
        private static KeyScriptData GetSceneKeyScriptData(RouterConfig routerConfig) => new()
        {
            keyTypeName = nameof(SceneKey),
            namespaceValue = "Managers.Router.Config",
            
            getPathValue = () => routerConfig.SceneKeysScriptPath,
            onPathCreated = (path) => 
            { 
                routerConfig.SceneKeysScriptPath = path; 
                EditorUtility.SetDirty(routerConfig); 
            },
            
            getDict = () => 
            {
                var scenes = routerConfig.Scenes;
                var result = scenes?.Where(x => !string.IsNullOrEmpty(x.Title))
                             .ToDictionary(x => x.Title, x => x.Id) ?? new Dictionary<string, string>();
                UnityEngine.Debug.Log($"[ScriptGen] Scene data collected: {result.Count} scenes");
                foreach (var scene in result)
                {
                    UnityEngine.Debug.Log($"[ScriptGen] Scene: {scene.Key} = {scene.Value}");
                }
                return result;
            }
        };
        
        private static KeyScriptData GetRoutKeyScriptData(RouterConfig routerConfig) => new()
        {
            keyTypeName = nameof(RoutKey),
            namespaceValue = "Managers.Router.Config",
            
            getPathValue = () => routerConfig.RoutKeysScriptPath,
            onPathCreated = (path) => 
            { 
                routerConfig.RoutKeysScriptPath = path; 
                EditorUtility.SetDirty(routerConfig); 
            },
            
            getDict = () => 
            {
                var routs = routerConfig.Routs;
                return routs?.Where(x => !string.IsNullOrEmpty(x.Title))
                            .ToDictionary(x => x.Title, x => x.Id) ?? new Dictionary<string, string>();
            }
        };
        
        private static KeyScriptData GetRoutArgsKeyScriptData(RouterConfig routerConfig) => new()
        {
            keyTypeName = nameof(RoutArgKey),
            namespaceValue = "Managers.Router.Config",
            
            getPathValue = () => routerConfig.RoutArgsScriptPath,
            onPathCreated = (path) => 
            { 
                routerConfig.RoutArgsScriptPath = path; 
                EditorUtility.SetDirty(routerConfig); 
            },
            
            getDict = () =>
            {
                var argKeysDict = new Dictionary<string, string>();
                var routs = routerConfig.Routs;
                if (routs != null)
                {
                    foreach (var group in routs)
                    {
                        var groupKey = group.Title;
                        if (!string.IsNullOrEmpty(groupKey))
                        {
                            var args = group.ArgsData;
                            if (args != null)
                            {
                                foreach (var rout in args)
                                {
                                    if (!string.IsNullOrEmpty(rout.Title))
                                    {
                                        var routKey = $"{groupKey}/{rout.Title}";
                                        argKeysDict[routKey] = rout.Id;
                                    }
                                }
                            }
                        }
                    }
                }
                return argKeysDict;
            }
        };
        
        private static KeyScriptData GetLoadingKeyScriptData(RouterConfig routerConfig) => new()
        {
            keyTypeName = nameof(LoadingScreenKey),
            namespaceValue = "Managers.Router.Config",
            
            getPathValue = () => routerConfig.LoadingsScriptPath,
            onPathCreated = (path) => 
            { 
                routerConfig.LoadingsScriptPath = path; 
                EditorUtility.SetDirty(routerConfig); 
            },
            
            getDict = () => 
            {
                var loadings = routerConfig.Loadings;
                return loadings?.Where(x => !string.IsNullOrEmpty(x.Title))
                               .ToDictionary(x => x.Title, x => x.Id) ?? new Dictionary<string, string>();
            }
        };
    }
}

#endif