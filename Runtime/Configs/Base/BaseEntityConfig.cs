﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Module.Utils.Configs;
using SimpleJSON;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    [ShowOdinSerializedPropertiesInInspector]
    public abstract class BaseEntityConfig<TTitleData, TData> : BaseConfig, IEntityConfig
        where TTitleData : BaseTitleData<TData>, new()
        where TData : BaseData, new()
    {
        [ShowInInspector, ListDrawerSettings(Expanded = true, ShowFoldout = false, ListElementLabelName = "@Title")]
        [OnValueChanged(nameof(OnDataListChanged))]
        public List<TTitleData> DataList { get; protected set; } = new();

        public string Name => GetType().Name;

        public TTitleData GetTitleDataById(string id) => DataList?.Find(item => item.EntityData?.Id == id);

        public IConfigData GetDataById(string id) => GetTitleDataById(id)?.EntityData;

        public IEnumerable<IConfigData> GetDataList() => DataList?.Select(item => item.EntityData as IConfigData) ?? Enumerable.Empty<IConfigData>();

        public TData GetDataWithComponent<T>() where T : class
        {
            return DataList?.FirstOrDefault(data => data.EntityData.HasComponent<T>())?.EntityData;
        }

        public IEnumerable GetValueDropdownList<T>()
        {
            var ids = new ValueDropdownList<T>();
            if (DataList != null)
            {
                foreach (var titleData in DataList)
                {
                    var instance = (T)Activator.CreateInstance(typeof(T), args: titleData.EntityData.Id.ToCharArray());
                    ids.Add(titleData.Title, instance);
                }
            }
            return ids;
        }

        public async UniTask LoadFromJSON(string json)
        {
            try
            {
                var array = await UniTask.RunOnThreadPool(() => JSONNode.Parse(json).AsArray);
                if (array?.Count > 0)
                {
                    JsonUtility.FromJsonOverwrite(array[0].ToString(), this);
                    
                    for (int i = 1; i < array.Count && i - 1 < DataList.Count; i++)
                    {
                        JsonUtility.FromJsonOverwrite(array[i].ToString(), DataList[i - 1].EntityData);
                    }
                    
#if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
#endif
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load config from JSON: {e.Message}");
            }
        }

#if UNITY_EDITOR
        [FoldoutGroup("Entity Management", expanded: false)]
        [Button("Add New Entity", ButtonSizes.Medium)]
        private void AddNewEntity()
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning("Cannot add entities during play mode!");
                return;
            }

            var newTitleData = new TTitleData();
            newTitleData.GenerateData(this);
            DataList.Add(newTitleData);
            EditorUtility.SetDirty(this);
        }

        [FoldoutGroup("Entity Management")]
        [Button("Clear Component From All", ButtonSizes.Small)]
        private void ClearComponentsFromUnits([ValueDropdown("@GetComponentTypeNames()")] string componentName)
        {
            if (string.IsNullOrEmpty(componentName)) return;
            
            int removedCount = 0;
            foreach (var titleData in DataList)
            {
                var component = titleData.EntityData.Components?.Find(item => item.GetType().Name.Equals(componentName));
                if (component != null)
                {
                    titleData.EntityData.Components.Remove(component);
                    removedCount++;
                    EditorUtility.SetDirty(titleData.EntityData);
                }
            }
            Debug.Log($"Removed {removedCount} components of type {componentName}");
        }
        
        private IEnumerable<string> GetComponentTypeNames()
        {
            return DataList?.SelectMany(td => td.EntityData.Components ?? new List<IBaseComponent>())
                          .Select(c => c.GetType().Name)
                          .Distinct() ?? Enumerable.Empty<string>();
        }

        [FoldoutGroup("Import/Export", expanded: false)]
        [HorizontalGroup("Import/Export/Buttons")]
        [Button("Export JSON", ButtonSizes.Medium)]
        private void SaveConfigToJSON()
        {
            var assetPath = EditorUtility.SaveFilePanel(
                "Export Config JSON",
                Application.dataPath,
                Name,
                "json");

            if (!string.IsNullOrEmpty(assetPath))
            {
                try
                {
                    var array = new JSONArray();
                    array.Add(JSONNode.Parse(JsonUtility.ToJson(this)));
                    foreach (var titleData in DataList)
                    {
                        array.Add(JSONNode.Parse(JsonUtility.ToJson(titleData.EntityData)));
                    }
                    File.WriteAllText(assetPath, array.ToString());
                    Debug.Log($"Config exported to: {assetPath}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to export config: {e.Message}");
                }
            }
        }

        [HorizontalGroup("Import/Export/Buttons")]
        [Button("Import JSON", ButtonSizes.Medium)]
        private void LoadConfigFromJSON()
        {
            var assetPath = EditorUtility.OpenFilePanel(
                "Import Config JSON",
                Application.dataPath,
                "json");

            if (!string.IsNullOrEmpty(assetPath))
            {
                var data = File.ReadAllText(assetPath);
                LoadFromJSON(data).Forget();
            }
        }

        public bool RemoveTitleData(ITitleData titleData)
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning("Cannot remove entities during play mode!");
                return false;
            }

            var success = DataList.Remove(titleData as TTitleData);
            if (success)
            {
                EditorUtility.SetDirty(this);
            }
            return success;
        }

        public bool CloneEntity(string entityId)
        {
            var sourceData = GetDataById(entityId) as TData;
            if (sourceData == null) return false;

            var clonedTitleData = new TTitleData();
            clonedTitleData.GenerateData(this, sourceData, true);
            DataList.Add(clonedTitleData);
            EditorUtility.SetDirty(this);
            return true;
        }
        
        private void OnDataListChanged()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(this);
            }
        }
#endif
    }

    public interface IEntityConfig
    {
        public string Name { get; }
        public IConfigData GetDataById(string id);
        public IEnumerable<IConfigData> GetDataList();
        public UniTask LoadFromJSON(string json);
        public IEnumerable GetValueDropdownList<T>();

#if UNITY_EDITOR
        public bool RemoveTitleData(ITitleData titleData);
        public bool CloneEntity(string entityId);
#endif
    }
}