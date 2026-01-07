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
    /// Editor-only helper class for script file generation in BaseRouterConfig.
    /// Требует ручного указания путей к файлам ключей.
    /// Поддерживает префикс для имён классов ключей.
    /// </summary>
    public static class RouterConfigScriptGenerator
    {
        public static bool UpdateSceneScriptFiles(BaseRouterConfig config)
        {
            var sceneKeyData = GetSceneKeyScriptData(config);
            return UpdateScriptFile(sceneKeyData, "Scene Keys");
        }
        
        public static bool UpdateRoutScriptFiles(BaseRouterConfig config)
        {
            var routKeyData = GetRoutKeyScriptData(config);
            var routArgsKeyData = GetRoutArgsKeyScriptData(config);
            
            var result1 = UpdateScriptFile(routKeyData, "Route Keys");
            var result2 = UpdateScriptFile(routArgsKeyData, "Route Args Keys");
            
            return result1 && result2;
        }
        
        public static bool UpdateLoadingScriptFiles(BaseRouterConfig config)
        {
            var loadingKeyData = GetLoadingKeyScriptData(config);
            return UpdateScriptFile(loadingKeyData, "Loading Keys");
        }

        public static bool UpdateAllScriptFiles(BaseRouterConfig config)
        {
            var result1 = UpdateSceneScriptFiles(config);
            var result2 = UpdateRoutScriptFiles(config);
            var result3 = UpdateLoadingScriptFiles(config);
            
            return result1 && result2 && result3;
        }

        /// <summary>
        /// Получить имя класса с учётом префикса
        /// </summary>
        private static string GetClassName(string baseTypeName, string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                return $"{baseTypeName}s";
            
            return $"{prefix}{baseTypeName}s";
        }
        
        /// <summary>
        /// Обновляет файл скрипта. Путь должен быть указан вручную.
        /// </summary>
        private static bool UpdateScriptFile(KeyScriptData scriptData, string displayName)
        {
            var currentPath = scriptData.getPathValue?.Invoke();
            
            if (string.IsNullOrEmpty(currentPath))
            {
                Debug.LogWarning($"[RouterConfig] Path for {displayName} is not set. Please specify the path manually in the config.");
                EditorUtility.DisplayDialog(
                    "Path Not Set", 
                    $"Path for {displayName} is not configured.\n\nPlease set the script path in the config's 'Script Generation' section.", 
                    "OK"
                );
                return false;
            }
            
            var fullPath = Path.Combine(Application.dataPath, currentPath.TrimStart('/', '\\'));
            
            // Создаём директорию если не существует
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            if (File.Exists(fullPath))
            {
                KeyScriptFileUtils.UpdateScriptFile(scriptData);
                Debug.Log($"[RouterConfig] {displayName} updated at: {currentPath}");
            }
            else
            {
                var assetPath = "Assets" + currentPath;
                KeyScriptFileUtils.CreateScriptFile(assetPath, scriptData);
                Debug.Log($"[RouterConfig] {displayName} created at: {currentPath}");
            }
            
            AssetDatabase.Refresh();
            return true;
        }


        #region Script Data Providers

        public static KeyScriptData GetSceneKeyScriptData(BaseRouterConfig config)
        {
            var prefix = config.KeysPrefix ?? "";
            var className = GetClassName(nameof(SceneKey), prefix);
            
            return new KeyScriptData
            {
                keyTypeName = nameof(SceneKey),
                customFileName = className,
                namespaceValue = "Managers.Router.Config",
                
                getPathValue = () => config.SceneKeysScriptPath,
                onPathCreated = path => 
                { 
                    config.SceneKeysScriptPath = path; 
                    EditorUtility.SetDirty(config); 
                },
                
                getDict = () => 
                {
                    var scenes = config.Scenes;
                    return scenes?.Where(x => !string.IsNullOrEmpty(x.Title))
                                 .ToDictionary(x => x.Title, x => x.Id) ?? new Dictionary<string, string>();
                }
            };
        }
        
        public static KeyScriptData GetRoutKeyScriptData(BaseRouterConfig config)
        {
            var prefix = config.KeysPrefix ?? "";
            var className = GetClassName(nameof(RoutKey), prefix);
            
            return new KeyScriptData
            {
                keyTypeName = nameof(RoutKey),
                customFileName = className,
                namespaceValue = "Managers.Router.Config",
                
                getPathValue = () => config.RoutKeysScriptPath,
                onPathCreated = path => 
                { 
                    config.RoutKeysScriptPath = path; 
                    EditorUtility.SetDirty(config); 
                },
                
                getDict = () => 
                {
                    var routs = config.Routs;
                    return routs?.Where(x => !string.IsNullOrEmpty(x.Title))
                                .ToDictionary(x => x.Title, x => x.Id) ?? new Dictionary<string, string>();
                }
            };
        }

        
        public static KeyScriptData GetRoutArgsKeyScriptData(BaseRouterConfig config)
        {
            var prefix = config.KeysPrefix ?? "";
            var className = GetClassName(nameof(RoutArgKey), prefix);
            
            return new KeyScriptData
            {
                keyTypeName = nameof(RoutArgKey),
                customFileName = className,
                namespaceValue = "Managers.Router.Config",
                
                getPathValue = () => config.RoutArgsScriptPath,
                onPathCreated = path => 
                { 
                    config.RoutArgsScriptPath = path; 
                    EditorUtility.SetDirty(config); 
                },
                
                getDict = () =>
                {
                    var argKeysDict = new Dictionary<string, string>();
                    var routs = config.Routs;
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
        }
        
        public static KeyScriptData GetLoadingKeyScriptData(BaseRouterConfig config)
        {
            var prefix = config.KeysPrefix ?? "";
            var className = GetClassName(nameof(LoadingScreenKey), prefix);
            
            return new KeyScriptData
            {
                keyTypeName = nameof(LoadingScreenKey),
                customFileName = className,
                namespaceValue = "Managers.Router.Config",
                
                getPathValue = () => config.LoadingsScriptPath,
                onPathCreated = path => 
                { 
                    config.LoadingsScriptPath = path; 
                    EditorUtility.SetDirty(config); 
                },
                
                getDict = () => 
                {
                    var loadings = config.Loadings;
                    return loadings?.Where(x => !string.IsNullOrEmpty(x.Title))
                                   .ToDictionary(x => x.Title, x => x.Id) ?? new Dictionary<string, string>();
                }
            };
        }

        #endregion
    }
}

#endif
