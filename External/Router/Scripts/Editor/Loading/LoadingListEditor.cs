using System.Collections.Generic;
using System.Linq;
using Managers.Router.Config;
using Managers.Router.Config.Loading;
using Module.InteractiveEditor;
using Module.InteractiveEditor.Configs;
using Module.Utils;
using Module.Utils.Configs;
using Module.Utils.Editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Submodules.Router.Editor
{
    public class LoadingListEditor
    {
        private readonly SerializedObject container;
        
        [ShowInInspector, ListDrawerSettings(
            ShowIndexLabels = true,
            DraggableItems = true,
            CustomAddFunction = nameof(AddLoading),
            CustomRemoveElementFunction = nameof(RemoveLoading)
        )]
        private List<LoadingScreenData> loadings => 
            container.targetObject.GetFieldValue<List<LoadingScreenData>>(RouterConfig.LoadingDataKey);

        public LoadingListEditor(SerializedObject container, string fieldName)
        {
            this.container = container;
        }
        
        public void Init()
        {
        }

        public void Close()
        {
            UpdateScriptFile();
        }
        
        public void DrawEditor()
        {
            if (!CheckScriptFile()) return;
            
            // Odin Inspector will automatically handle the list drawing
            // with the attributes defined above
        }

        private void AddLoading()
        {
            var instance = ScriptableEntity.Create<LoadingScreenData>();
                
            Undo.RecordObject(container.targetObject, "Add Loading");
                
            container.targetObject.AddToList(RouterConfig.LoadingDataKey, instance);
                
            AssetDatabase.AddObjectToAsset(instance, container.targetObject);
                
            Undo.RegisterCreatedObjectUndo(instance, "Add Loading");
                
            EditorUtility.SetDirty(container.targetObject);
                
            AssetDatabase.SaveAssets();
        }
        
        private void RemoveLoading(int index)
        {
            var loadingsList = container.targetObject.GetFieldValue<List<LoadingScreenData>>(RouterConfig.LoadingDataKey);

            if (loadingsList != null && index >= 0 && index < loadingsList.Count)
            {
                Undo.RecordObject(container.targetObject, "Delete Loading");
                    
                var item = loadingsList[index];
                container.targetObject.RemoveFromList(RouterConfig.LoadingDataKey, item);
                    
                Undo.DestroyObjectImmediate(item);
                    
                AssetDatabase.SaveAssets();
            }
        }
        
        private bool CheckScriptFile()
        {
            return KeyScriptFileUtils.CheckScriptFile(GetKeyScriptData());
        }
        
        private void UpdateScriptFile()
        {
            KeyScriptFileUtils.UpdateScriptFile(GetKeyScriptData());
        }
        
        private KeyScriptData GetKeyScriptData() => new()
        {
            keyTypeName = nameof(LoadingScreenKey),
            namespaceValue = "Managers.Router.Config",

            getPathValue = () => container.targetObject.GetFieldValue<string>(RouterConfig.LoadingsScriptPathKey),
            onPathCreated = (path) => container.targetObject.SetFieldValue(RouterConfig.LoadingsScriptPathKey, path),

            getDict = () =>
            {
                return container.targetObject
                    .GetFieldValue<List<LoadingScreenData>>(RouterConfig.LoadingDataKey)
                    .Where(x=> !string.IsNullOrEmpty(x.Title))
                    .ToDictionary(x => x.Title, x => x.Id);
            }
        };
    }
}