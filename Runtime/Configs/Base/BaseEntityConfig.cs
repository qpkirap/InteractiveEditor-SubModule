using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Module.Utils.Configs;
using SimpleJSON;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public abstract class BaseEntityConfig<TTitleData, TData, TComponent> : BaseConfig, IEntityConfig
        where TTitleData : BaseTitleData<TData, TComponent>
        where TData : BaseData<TComponent>
        where TComponent : IBaseComponent
    {
        [field: SerializeField, HideInInspector]
        public List<TTitleData> DataList { get; protected set; }

        public string Name => GetType().Name;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TTitleData GetTitleDataById(string id)
        {
            return DataList.Find(item => item.EntityData?.Id == id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IConfigData GetDataById(string id)
        {
            return GetTitleDataById(id)?.EntityData;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<IConfigData> GetDataList()
        {
            return DataList.Select(item => item.EntityData as IConfigData);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TData GetDataWithComponent<T>()
            where T : class
        {
            foreach (var data in DataList)
            {
                if (data.EntityData.HasComponent<T>())
                {
                    return data.EntityData;
                }
            }

            return null;
        }

        public IEnumerable GetValueDropdownList<T>()
        {
            var ids = new ValueDropdownList<T>();

            foreach (var titleData in DataList)
            {
                var instance = (T)Activator.CreateInstance(typeof(T), args: titleData.EntityData.Id.ToCharArray());
                ids.Add(titleData.Title, instance);
            }

            return ids;
        }

        public async UniTask LoadFromJSON(string json)
        {
            var array = await UniTask.RunOnThreadPool(() => JSONNode.Parse(json).AsArray);
            for (int i = 0; i < array.Count; i++)
            {
                if (i == 0)
                {
                    JsonUtility.FromJsonOverwrite(array[i].ToString(), this);

#if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
#endif
                }
                else
                {
                    JsonUtility.FromJsonOverwrite(array[i].ToString(), DataList[i - 1].EntityData);

#if UNITY_EDITOR
                    EditorUtility.SetDirty(DataList[i - 1].EntityData);
#endif
                }
            }
        }

#if UNITY_EDITOR
        public bool RemoveTitleData(ITitleData titleData)
        {
            if (Application.isPlaying)
            {
                Debug.Log(
                    "ALARM! Ты делаешь что-то неправильное, почему удаляешь элементы конфига при запущенной игре?");
                return false;
            }

            var success = DataList.Remove(titleData as TTitleData);
            if (success)
            {
                EditorUtility.SetDirty(this);
                return true;
            }

            return false;
        }

        public bool CloneEntity(string entityId)
        {
            var unitData = GetDataById(entityId) as TData;
            if (unitData == null) return false;

            var unitTitleData = Activator.CreateInstance<TTitleData>();
            unitTitleData.GenerateData(this, unitData, true);
            DataList.Add(unitTitleData);

            EditorUtility.SetDirty(this);

            return true;
        }

        [Title("Стандартный функционал", TitleAlignment = TitleAlignments.Centered)]
        [Button("Добавить новую сущность", ButtonSizes.Medium), PropertyOrder(1)]
        protected void AddNewEntity()
        {
            var unitTitleData = Activator.CreateInstance<TTitleData>();
            unitTitleData.GenerateData(this);
            DataList.Add(unitTitleData);

            EditorUtility.SetDirty(this);
        }

        [Button("Очистить все сущности от компонента", ButtonSizes.Medium), PropertyOrder(1)]
        protected void ClearComponentsFromUnits(string componentName)
        {
            foreach (var titleData in DataList)
            {
                var component = titleData.EntityData.Components.Find(item => item.GetType().Name.Equals(componentName));
                if (component != null)
                {
                    titleData.EntityData.Components.Remove(component);
                }

                EditorUtility.SetDirty(titleData.EntityData);
            }
        }

        [Button("Сохранить конфиг в json", ButtonSizes.Medium), PropertyOrder(1), HorizontalGroup()]
        protected void SaveConfigToJSON()
        {
            var assetPath = EditorUtility.SaveFilePanel(
                title: $"Куда сохранить файл json",
                directory: Application.dataPath,
                defaultName: Name,
                extension: "json");

            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            var array = new JSONArray();
            array.Add(JSONNode.Parse(JsonUtility.ToJson(this)));
            foreach (var titleData in DataList)
            {
                array.Add(JSONNode.Parse(JsonUtility.ToJson(titleData.EntityData)));
            }

            File.WriteAllText(assetPath, array.ToString());
        }

        [Button("Загрузить конфиг из json", ButtonSizes.Medium), PropertyOrder(1), HorizontalGroup()]
        protected void LoadConfigFromJSON()
        {
            var assetPath = EditorUtility.OpenFilePanel(
                title: $"Открыть файл json конфига",
                directory: Application.dataPath,
                extension: "json");

            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            var data = File.ReadAllText(assetPath);
            LoadFromJSON(data);
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