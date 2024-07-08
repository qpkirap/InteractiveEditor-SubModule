using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Module.InteractiveEditor.Configs.Utils
{
    public static class ConfigUtils
    {
        public static IEnumerable GetDropdownList<TConfig, TValue>()
            where TConfig : Object, IEntityConfig
        {
#if UNITY_EDITOR
            var asset = LoadAsset<TConfig>();
            if (asset != null)
            {
                return asset.GetValueDropdownList<TValue>();
            }
#endif
            return null;
        }

#if UNITY_EDITOR
        public static void ShowInspector(Object objectToShow)
        {
            var oldSelection = Selection.activeObject;
            Selection.activeObject = objectToShow;

            var inspectorWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");

            MethodInfo method = typeof(EditorWindow).GetMethod(nameof(EditorWindow.CreateWindow), new[] { typeof(System.Type[]) });
            MethodInfo generic = method.MakeGenericMethod(inspectorWindowType);

            var window = generic.Invoke(null, new object[] { new System.Type[] { } });

            PropertyInfo propertyInfo = inspectorWindowType.GetProperty("isLocked");
            bool value = (bool)propertyInfo.GetValue(window, null);
            propertyInfo.SetValue(window, true, null);

            Selection.activeObject = oldSelection;
        }

        public static T LoadAsset<T>(Action onError = null, bool quiet = true) where T : Object
        {
            return LoadAsset(typeof(T), onError, quiet) as T;
        }

        public static T LoadAsset<T>(string assetGUID) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(assetGUID));
        }

        public static object LoadAsset(string assetGUID, Type assetType)
        {
            return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assetGUID), assetType);
        }

        public static object LoadAsset(Type assetType, Action onError = null, bool quiet = true)
        {
            var typeName = assetType.Name;

            var assetGUID = AssetDatabase
                .FindAssets($"t:{typeName}")
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(assetGUID))
            {
                return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assetGUID), assetType);
            }
            else
            {
                if (!quiet)
                {
                    Debug.LogError($"Ассет {typeName} не найден");
                }

                onError?.Invoke();

                return null;
            }
        }

        public static T LoadAssetByName<T>(string assetName, string filterPath = null)
            where T : Object
        {
            var typeName = typeof(T).Name;
            var filter = CreateFilterPath(filterPath);

            return AssetDatabase
                .FindAssets($"t:{typeName} {assetName}", filter)
                .Select(guid => LoadAsset<T>(guid))
                .FirstOrDefault();
        }

        private static string[] CreateFilterPath(string filterPath)
        {
            return string.IsNullOrEmpty(filterPath) ? null : new string[] { filterPath };
        }
#endif
    }
}