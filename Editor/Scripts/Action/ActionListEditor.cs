using System;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Runtime;
using Module.Utils;
using Module.Utils.Configs;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Module.InteractiveEditor.Editor
{
    [CustomEditor(typeof(ActionTask))]
    public class ActionListEditor : UnityEditor.Editor
    {
        private Object container;
        private string fieldAction;

        public virtual void Init(Object container, string fieldAction)
        {
            this.container = container;
            this.fieldAction = fieldAction;
        }


        private ActionTask CreateNode(Type type)
        {
            var instance = ScriptableEntity.Create<ActionTask>(type);

            Undo.RecordObject(container, "Create Action");

            container.AddToList(BaseActionNode.TasksKey, instance);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(instance, container);
            }

            Undo.RegisterCreatedObjectUndo(instance, "Create Action");

            EditorUtility.SetDirty(container);

            AssetDatabase.SaveAssets();

            return instance;
        }
    }
}