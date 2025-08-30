using System.Collections.Generic;
using System.Linq;
using Managers.Router.Config;
using Module.Utils;
using Module.Utils.Configs;
using Module.Utils.Editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Submodules.Router.Editor
{
    public class RoutsListEditor
    {
        private readonly SerializedObject container;
        
        [ShowInInspector, ListDrawerSettings(
            ShowIndexLabels = true,
            DraggableItems = true,
            CustomAddFunction = nameof(AddRoute),
            CustomRemoveElementFunction = nameof(RemoveRoute)
        )]
        private List<RoutData> routes => 
            container.targetObject.GetFieldValue<List<RoutData>>(RouterConfig.RoutDataKey);

        public RoutsListEditor(SerializedObject container, string fieldName)
        {
            this.container = container;
        }

        public void Init()
        {
        }

        public void Close()
        {
            UpdateSceneScriptFile();
        }
        
        public void DrawEditor()
        {
            if (!CheckSceneScriptFile()) return;
            
            // Odin Inspector will automatically handle the list drawing
            // with the attributes defined above
        }
        
        private void AddRoute()
        {
            var instance = ScriptableEntity.Create<RoutData>();
                
            Undo.RecordObject(container.targetObject, "Add route");
            
            container.targetObject.AddToList(RouterConfig.RoutDataKey, instance);
                
            AssetDatabase.AddObjectToAsset(instance, container.targetObject);
                
            Undo.RegisterCreatedObjectUndo(instance, "Add route");
                
            EditorUtility.SetDirty(container.targetObject);
                
            AssetDatabase.SaveAssets();
        }
        
        private void RemoveRoute(int index)
        {
            var routesList = container.targetObject.GetFieldValue<List<RoutData>>(RouterConfig.RoutDataKey);

            if (routesList != null && index >= 0 && index < routesList.Count)
            {
                Undo.RecordObject(container.targetObject, "Delete Route");
                    
                var item = routesList[index];
                container.targetObject.RemoveFromList(RouterConfig.RoutDataKey, item);
                    
                Undo.DestroyObjectImmediate(item);
                    
                AssetDatabase.SaveAssets();
            }
        }
        
        private bool CheckSceneScriptFile()
        {
            return KeyScriptFileUtils.CheckScriptFile(GetRoutKeyScriptData())
                   && KeyScriptFileUtils.CheckScriptFile(GetRoutArgsKeyScriptData());
        }
        
        private void UpdateSceneScriptFile()
        {
            KeyScriptFileUtils.UpdateScriptFile(GetRoutKeyScriptData());
            KeyScriptFileUtils.UpdateScriptFile(GetRoutArgsKeyScriptData());
        }
        
        private KeyScriptData GetRoutKeyScriptData() => new()
        {
            keyTypeName = nameof(RoutKey),
            namespaceValue = "Managers.Router.Config",

            getPathValue = () => container.targetObject.GetFieldValue<string>(RouterConfig.RoutKeysScriptPathKey),
            onPathCreated = (path) => container.targetObject.SetFieldValue(RouterConfig.RoutKeysScriptPathKey, path),

            getDict = () =>
            {
                return container.targetObject
                    .GetFieldValue<List<RoutData>>(RouterConfig.RoutDataKey)
                    .Where(x=> !string.IsNullOrEmpty(x.Title))
                    .ToDictionary(x => x.Title, x => x.Id);
            }
        };
        
        private KeyScriptData GetRoutArgsKeyScriptData() => new()
        {
            keyTypeName = nameof(RoutArgKey),
            namespaceValue = "Managers.Router.Config",

            getPathValue = () => container.targetObject.GetFieldValue<string>(RouterConfig.RoutArgsScriptPathKey),
            onPathCreated = (path) => container.targetObject.SetFieldValue(RouterConfig.RoutArgsScriptPathKey, path),

            getDict = () =>
            {
                var argKeysDict = new Dictionary<string, string>();
                var routGroups = container.targetObject.GetFieldValue<List<RoutData>>(RouterConfig.RoutDataKey);
                
                foreach (var group in routGroups)
                {
                    var groupKey = group.Title;

                    foreach (var rout in group.GetFieldValue<List<RoutArgData>>(RoutData.ArgDataKey))
                    {
                        var routKey = $"{groupKey}/{rout.Title}";
                        
                        var argId = rout.Id;

                        argKeysDict.Add(routKey, argId);
                    }
                }

                return argKeysDict;
            }
        };
    }
}