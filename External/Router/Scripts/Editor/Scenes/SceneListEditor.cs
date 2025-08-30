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
    public class SceneListEditor
    {
        private readonly SerializedObject container;
        
        [ShowInInspector, ListDrawerSettings(
            ShowIndexLabels = true,
            DraggableItems = true,
            CustomAddFunction = nameof(AddScene),
            CustomRemoveElementFunction = nameof(RemoveScene)
        )]
        private List<SceneData> scenes => 
            container.targetObject.GetFieldValue<List<SceneData>>(RouterConfig.SceneDataKey);

        public SceneListEditor(SerializedObject container, string fieldName)
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

        private void AddScene()
        {
            var instance = ScriptableEntity.Create<SceneData>();
                
            Undo.RecordObject(container.targetObject, "Add scene");
                
            container.targetObject.AddToList(RouterConfig.SceneDataKey, instance);
                
            AssetDatabase.AddObjectToAsset(instance, container.targetObject);
                
            Undo.RegisterCreatedObjectUndo(instance, "Add scene");
                
            EditorUtility.SetDirty(container.targetObject);
                
            AssetDatabase.SaveAssets();
        }
        
        private void RemoveScene(int index)
        {
            var scenesList = container.targetObject.GetFieldValue<List<SceneData>>(RouterConfig.SceneDataKey);

            if (scenesList != null && index >= 0 && index < scenesList.Count)
            {
                Undo.RecordObject(container.targetObject, "Delete scene");
                    
                var item = scenesList[index];
                container.targetObject.RemoveFromList(RouterConfig.SceneDataKey, item);
                    
                Undo.DestroyObjectImmediate(item);
                    
                AssetDatabase.SaveAssets();
            }
        }
        
        private bool CheckSceneScriptFile()
        {
            return KeyScriptFileUtils.CheckScriptFile(GetSceneKeyScriptData());
        }
        
        private void UpdateSceneScriptFile()
        {
            KeyScriptFileUtils.UpdateScriptFile(GetSceneKeyScriptData());
        }
        
        private KeyScriptData GetSceneKeyScriptData() => new()
        {
            keyTypeName = nameof(SceneKey),
            namespaceValue = "Managers.Router.Config",

            getPathValue = () => container.targetObject.GetFieldValue<string>(RouterConfig.SceneKeysScriptPathKey),
            onPathCreated = (path) => container.targetObject.SetFieldValue(RouterConfig.SceneKeysScriptPathKey, path),

            getDict = () =>
            {
                return container.targetObject
                    .GetFieldValue<List<SceneData>>(RouterConfig.SceneDataKey)
                    .Where(x=> !string.IsNullOrEmpty(x.Title))
                    .ToDictionary(x => x.Title, x => x.Id);
            }
        };
    }
}