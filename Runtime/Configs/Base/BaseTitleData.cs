using System;
using System.IO;
using System.Linq;
using Module.InteractiveEditor.Configs.Utils;
using Module.Utils.Configs;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public abstract class BaseTitleData<TData, TComponent> : ITitleData
        where TData : BaseData<TComponent>
        where TComponent : IBaseComponent
    {
        [ShowInInspector, LabelText("")]
        public string Title => EntityData == null ? string.Empty : EntityData.Prefix + EntityData.Title;

        [field: SerializeField, LabelText("")]
        public TData EntityData { get; internal set; }

#if UNITY_EDITOR
        internal void GenerateData(UnityEngine.Object config, TData source = null, bool forceGenerateId = false)
        {
            var configPath = AssetDatabase.GetAssetPath(config);
            var directoryPath = $"{Path.GetDirectoryName(configPath)}/Data";
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            var assetPath = $"{directoryPath}/Data_TEMP.asset";
            EntityData = Activator.CreateInstance<TData>();
            AssetDatabase.CreateAsset(EntityData, assetPath);
            EntityData.ResetToDefaultComponents();

            if (source != null)
                EditorUtility.CopySerialized(source, EntityData);

            EntityData.GenerateId(forceGenerateId);
        }

        [Button("Удалить"), ShowIf(nameof(EntityData))]
        private void RemoveEntity()
        {
            var config = GetParentConfig();
            if (config != null)
            {
                if (config.RemoveTitleData(this))
                {
                    var path = AssetDatabase.GetAssetPath(EntityData);
                    AssetDatabase.DeleteAsset(path);
                }
                else
                {
                    Debug.Log("ALARM! Не получилось удалить заголовок!");
                }
            }
            else
            {
                Debug.Log("ALARM! В родительской папке не найден конфиг!");
            }
        }

        [Button("Клонировать"), ShowIf(nameof(EntityData))]
        private void CloneEntity()
        {
            var config = GetParentConfig();
            if (config != null)
            {
                if (!config.CloneEntity(EntityData.Id))
                {
                    Debug.Log("ALARM! Не получилось клонировать! В родительском конфиге не найден исходный элемент!");
                }
            }
            else
            {
                Debug.Log("ALARM! В родительской папке не найден конфиг!");
            }
        }

        [Button("Показать"), ShowIf(nameof(EntityData))]
        private void ShowEntity()
        {
            if (EntityData != null)
            {
                foreach (var defaultComponent in EntityData.DefaultComponents)
                {
                    var any = EntityData.Components.Any(item => item.GetType() == defaultComponent.GetType());
                    if (!any)
                    {
                        EntityData.Components.Add(defaultComponent);

                        Debug.Log($"[HELPER] Вы забыли добавить компонент {defaultComponent.GetType().Name}, мы добавили его за вас, не забудьте его настроить!");
                    }
                }

                EditorUtility.SetDirty(EntityData);
            }

            ConfigUtils.ShowInspector(EntityData);
        }
        
        private IEntityConfig GetParentConfig()
        {
            var path = AssetDatabase.GetAssetPath(EntityData);
            var directoryName = Path.GetDirectoryName(path);
            var configDirectory = directoryName.Replace($"{Path.DirectorySeparatorChar}Data", "");
            var assets = AssetDatabase.FindAssets("t: BaseConfig", new string[1] { configDirectory });
            if (assets.Length > 0)
            {
                var baseConfig = AssetDatabase.LoadAssetAtPath<BaseConfig>(AssetDatabase.GUIDToAssetPath(assets[0]));
                return baseConfig as IEntityConfig;
            }

            return null;
        }
#endif
    }
    
    public interface ITitleData
    {
    }
}