﻿using System;
using System.IO;
using System.Linq;
using Module.InteractiveEditor.Configs.Utils;
using Module.Utils.Configs;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    [System.Serializable]
    public abstract class BaseTitleData<TData> : ITitleData
        where TData : BaseData, new()
    {
        [ShowInInspector, DisplayAsString, LabelText("")]
        public string Title => EntityData?.Prefix + EntityData?.Title ?? string.Empty;

        [InlineEditor(InlineEditorModes.GUIAndHeader), LabelText("")]
        public TData EntityData;

#if UNITY_EDITOR
        internal void GenerateData(UnityEngine.Object config, TData source = null, bool forceGenerateId = false)
        {
            try
            {
                var configPath = AssetDatabase.GetAssetPath(config);
                var directoryPath = Path.Combine(Path.GetDirectoryName(configPath), "Data");
                
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                var assetPath = Path.Combine(directoryPath, "Data_TEMP.asset");
                EntityData = new TData();
                AssetDatabase.CreateAsset(EntityData, assetPath);
                EntityData.ResetToDefaultComponents();

                if (source != null)
                    EditorUtility.CopySerialized(source, EntityData);

                EntityData.GenerateId(forceGenerateId);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to generate data: {e.Message}");
            }
        }

        [FoldoutGroup("Entity Actions", expanded: false)]
        [Button("Remove Entity", ButtonSizes.Small), ShowIf(nameof(EntityData))]
        private void RemoveEntity()
        {
            var config = GetParentConfig();
            if (config != null)
            {
                if (config.RemoveTitleData(this))
                {
                    var path = AssetDatabase.GetAssetPath(EntityData);
                    if (!string.IsNullOrEmpty(path))
                    {
                        AssetDatabase.DeleteAsset(path);
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to remove entity title data!");
                }
            }
            else
            {
                Debug.LogWarning("Parent config not found!");
            }
        }

        [FoldoutGroup("Entity Actions")]
        [Button("Clone Entity", ButtonSizes.Small), ShowIf(nameof(EntityData))]
        private void CloneEntity()
        {
            var config = GetParentConfig();
            if (config != null)
            {
                if (!config.CloneEntity(EntityData.Id))
                {
                    Debug.LogWarning("Failed to clone entity! Source not found in parent config!");
                }
            }
            else
            {
                Debug.LogWarning("Parent config not found!");
            }
        }

        [FoldoutGroup("Entity Actions")]
        [Button("Show Entity", ButtonSizes.Small), ShowIf(nameof(EntityData))]
        private void ShowEntity()
        {
            if (EntityData != null)
            {
                // Auto-add missing default components
                foreach (var defaultComponent in EntityData.DefaultComponents)
                {
                    var hasComponent = EntityData.Components.Any(item => item.GetType() == defaultComponent.GetType());
                    if (!hasComponent)
                    {
                        EntityData.Components.Add(defaultComponent);
                        Debug.Log($"[AUTO-FIX] Added missing component: {defaultComponent.GetType().Name}");
                    }
                }

                EditorUtility.SetDirty(EntityData);
                ConfigUtils.ShowInspector(EntityData);
            }
        }
        private IEntityConfig GetParentConfig()
        {
            if (EntityData == null) return null;
            
            try
            {
                var path = AssetDatabase.GetAssetPath(EntityData);
                var directoryName = Path.GetDirectoryName(path);
                var configDirectory = directoryName?.Replace($"{Path.DirectorySeparatorChar}Data", "");
                
                if (!string.IsNullOrEmpty(configDirectory))
                {
                    var assets = AssetDatabase.FindAssets("t: BaseConfig", new[] { configDirectory });
                    if (assets.Length > 0)
                    {
                        var baseConfig = AssetDatabase.LoadAssetAtPath<BaseConfig>(AssetDatabase.GUIDToAssetPath(assets[0]));
                        return baseConfig as IEntityConfig;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error finding parent config: {e.Message}");
            }

            return null;
        }
#endif
    }
    
    public interface ITitleData
    {
    }
}