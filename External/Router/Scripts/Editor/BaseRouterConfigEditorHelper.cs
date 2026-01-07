#if UNITY_EDITOR

using System.Collections.Generic;
using Managers.Router.Config.Loading;
using Module.Utils.Configs;
using UnityEditor;

namespace Managers.Router.Config
{
    /// <summary>
    /// Базовый хелпер для редактирования BaseRouterConfig.
    /// Содержит общую логику для всех конфигов роутера.
    /// </summary>
    public static class BaseRouterConfigEditorHelper
    {
        #region Scene Data

        public static void CreateNewSceneData(BaseRouterConfig config)
        {
            var instance = ScriptableEntity.Create<SceneData>();
            config.Scenes ??= new List<SceneData>();
            config.Scenes.Add(instance);
            SaveAsSubAsset(instance, config);
            
            RouterConfigScriptGenerator.UpdateSceneScriptFiles(config);
        }

        public static void RemoveSceneData(BaseRouterConfig config, SceneData element)
        {
            if (config.Scenes != null && element != null)
            {
                config.Scenes.Remove(element);
                RemoveSubAsset(element);
                
                RouterConfigScriptGenerator.UpdateSceneScriptFiles(config);
            }
        }

        #endregion

        #region Rout Data

        public static void CreateNewRoutData(BaseRouterConfig config)
        {
            var instance = ScriptableEntity.Create<RoutData>();
            config.Routs ??= new List<RoutData>();
            config.Routs.Add(instance);
            SaveAsSubAsset(instance, config);
            
            RouterConfigScriptGenerator.UpdateRoutScriptFiles(config);
        }

        public static void RemoveRoutData(BaseRouterConfig config, RoutData element)
        {
            if (config.Routs != null && element != null)
            {
                config.Routs.Remove(element);
                RemoveSubAsset(element);
                
                RouterConfigScriptGenerator.UpdateRoutScriptFiles(config);
            }
        }

        #endregion

        #region Loading Data

        public static void CreateNewLoadingData(BaseRouterConfig config)
        {
            var instance = ScriptableEntity.Create<LoadingScreenData>();
            config.Loadings ??= new List<LoadingScreenData>();
            config.Loadings.Add(instance);
            SaveAsSubAsset(instance, config);
            
            RouterConfigScriptGenerator.UpdateLoadingScriptFiles(config);
        }

        public static void RemoveLoadingData(BaseRouterConfig config, LoadingScreenData element)
        {
            if (config.Loadings != null && element != null)
            {
                config.Loadings.Remove(element);
                RemoveSubAsset(element);
                
                RouterConfigScriptGenerator.UpdateLoadingScriptFiles(config);
            }
        }

        #endregion

        #region Manual Update Methods

        public static void UpdateSceneKeys(BaseRouterConfig config)
        {
            SyncScriptPaths(config);
            RouterConfigScriptGenerator.UpdateSceneScriptFiles(config);
            UnityEngine.Debug.Log($"[{config.name}] Scene keys updated");
        }

        public static void UpdateRoutKeys(BaseRouterConfig config)
        {
            SyncScriptPaths(config);
            RouterConfigScriptGenerator.UpdateRoutScriptFiles(config);
            UnityEngine.Debug.Log($"[{config.name}] Route keys updated");
        }

        public static void UpdateLoadingKeys(BaseRouterConfig config)
        {
            SyncScriptPaths(config);
            RouterConfigScriptGenerator.UpdateLoadingScriptFiles(config);
            UnityEngine.Debug.Log($"[{config.name}] Loading keys updated");
        }

        public static void UpdateAllKeys(BaseRouterConfig config)
        {
            SyncScriptPaths(config);
            RouterConfigScriptGenerator.UpdateAllScriptFiles(config);
            UnityEngine.Debug.Log($"[{config.name}] All keys updated");
        }

        /// <summary>
        /// Синхронизирует пути к файлам с текущим префиксом.
        /// Переименовывает файлы если имена не соответствуют префиксу.
        /// </summary>
        public static void SyncScriptPaths(BaseRouterConfig config)
        {
            var prefix = config.KeysPrefix ?? "";
            var changed = false;

            var newScenePath = SyncSinglePath(config.SceneKeysScriptPath, "SceneKey", prefix);
            if (newScenePath != config.SceneKeysScriptPath)
            {
                config.SceneKeysScriptPath = newScenePath;
                changed = true;
            }

            var newRoutPath = SyncSinglePath(config.RoutKeysScriptPath, "RoutKey", prefix);
            if (newRoutPath != config.RoutKeysScriptPath)
            {
                config.RoutKeysScriptPath = newRoutPath;
                changed = true;
            }

            var newArgsPath = SyncSinglePath(config.RoutArgsScriptPath, "RoutArgKey", prefix);
            if (newArgsPath != config.RoutArgsScriptPath)
            {
                config.RoutArgsScriptPath = newArgsPath;
                changed = true;
            }

            var newLoadingPath = SyncSinglePath(config.LoadingsScriptPath, "LoadingScreenKey", prefix);
            if (newLoadingPath != config.LoadingsScriptPath)
            {
                config.LoadingsScriptPath = newLoadingPath;
                changed = true;
            }

            if (changed)
            {
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
                UnityEngine.Debug.Log($"[{config.name}] Script paths synchronized with prefix: '{prefix}'");
            }
        }

        /// <summary>
        /// Синхронизирует один путь с префиксом.
        /// Если файл существует но имя не соответствует — переименовывает.
        /// </summary>
        private static string SyncSinglePath(string currentPath, string baseTypeName, string prefix)
        {
            if (string.IsNullOrEmpty(currentPath))
                return currentPath;

            var expectedFileName = GetNewFileName(baseTypeName, prefix);
            var currentFileName = System.IO.Path.GetFileName(currentPath);

            // Если имя файла уже соответствует префиксу — ничего не делаем
            if (currentFileName == expectedFileName)
                return currentPath;

            // Переименовываем файл
            return RenameScriptFile(currentPath, baseTypeName, prefix);
        }

        /// <summary>
        /// Вызывается при изменении префикса ключей.
        /// Переименовывает существующие файлы скриптов и обновляет пути.
        /// </summary>
        public static void OnKeysPrefixChanged(BaseRouterConfig config)
        {
            SyncScriptPaths(config);
        }

        /// <summary>
        /// Переименовывает файл скрипта с учётом нового префикса.
        /// </summary>
        private static string RenameScriptFile(string currentPath, string baseTypeName, string prefix)
        {
            if (string.IsNullOrEmpty(currentPath))
                return currentPath;

            var oldAssetPath = "Assets" + currentPath;
            
            // Проверяем существует ли файл
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(oldAssetPath);
            if (asset == null)
            {
                // Файл не существует, просто обновляем путь
                var directory = System.IO.Path.GetDirectoryName(currentPath);
                var newFileName = GetNewFileName(baseTypeName, prefix);
                return string.IsNullOrEmpty(directory) 
                    ? newFileName 
                    : System.IO.Path.Combine(directory, newFileName).Replace('\\', '/');
            }

            var directory2 = System.IO.Path.GetDirectoryName(currentPath);
            var newFileName2 = GetNewFileName(baseTypeName, prefix);
            var newPath = string.IsNullOrEmpty(directory2) 
                ? newFileName2 
                : System.IO.Path.Combine(directory2, newFileName2).Replace('\\', '/');
            
            var newAssetPath = "Assets" + newPath;
            
            // Если путь не изменился, ничего не делаем
            if (oldAssetPath == newAssetPath)
                return currentPath;

            // Переименовываем файл
            var error = AssetDatabase.RenameAsset(oldAssetPath, System.IO.Path.GetFileNameWithoutExtension(newFileName2));
            
            if (!string.IsNullOrEmpty(error))
            {
                UnityEngine.Debug.LogError($"[RouterConfig] Failed to rename {oldAssetPath}: {error}");
                return currentPath;
            }
            
            UnityEngine.Debug.Log($"[RouterConfig] Renamed: {oldAssetPath} -> {newAssetPath}");
            return newPath;
        }

        private static string GetNewFileName(string baseTypeName, string prefix)
        {
            return string.IsNullOrEmpty(prefix) 
                ? $"{baseTypeName}s.cs" 
                : $"{prefix}{baseTypeName}s.cs";
        }

        #endregion

        #region Asset Helpers

        private static void SaveAsSubAsset(ScriptableEntity instance, BaseRouterConfig config)
        {
            AssetDatabase.AddObjectToAsset(instance, config);
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        private static void RemoveSubAsset(ScriptableEntity element)
        {
            Undo.DestroyObjectImmediate(element);
            AssetDatabase.SaveAssets();
        }

        #endregion
    }
}

#endif
