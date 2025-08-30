using System.Collections.Generic;
using System.Linq;
using Managers.Router.Config;
using Module.Utils;
using Module.Utils.Configs;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Submodules.Router.Editor
{
    public class RoutArgsListEditor
    {
        private readonly SerializedObject container;
        
        [ShowInInspector, ListDrawerSettings(
            ShowIndexLabels = true,
            DraggableItems = true,
            CustomAddFunction = nameof(AddArg),
            CustomRemoveElementFunction = nameof(RemoveArg)
        )]
        private List<RoutArgData> args => 
            container.targetObject.GetFieldValue<List<RoutArgData>>(RoutData.ArgDataKey);
        
        public RoutArgsListEditor(SerializedObject container, string fieldName)
        {
            this.container = container;
        }

        public void Init()
        {
        }

        public void Close()
        {
        }
        
        public void DrawEditor()
        {
            // Odin Inspector will automatically handle the list drawing
            // with the attributes defined above
        }

        private void AddArg()
        {
            var instance = ScriptableEntity.Create<RoutArgData>();
                
            Undo.RecordObject(container.targetObject, "Add arg");
            
            container.targetObject.AddToList(RoutData.ArgDataKey, instance);
                
            AssetDatabase.AddObjectToAsset(instance, container.targetObject);
                
            Undo.RegisterCreatedObjectUndo(instance, "Add arg");
                
            EditorUtility.SetDirty(container.targetObject);
                
            AssetDatabase.SaveAssets();
        }
        
        private void RemoveArg(int index)
        {
            var argsList = container.targetObject.GetFieldValue<List<RoutArgData>>(RoutData.ArgDataKey);

            if (argsList != null && index >= 0 && index < argsList.Count)
            {
                Undo.RecordObject(container.targetObject, "Delete arg");
                    
                var item = argsList[index];
                container.targetObject.RemoveFromList(RoutData.ArgDataKey, item);
                    
                Undo.DestroyObjectImmediate(item);
                    
                AssetDatabase.SaveAssets();
            }
        }
    }
}