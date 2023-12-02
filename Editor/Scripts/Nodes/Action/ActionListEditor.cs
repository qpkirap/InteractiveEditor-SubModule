using System;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Runtime;
using Module.Utils;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Component = Module.InteractiveEditor.Runtime.Component;

namespace Module.InteractiveEditor.Editor
{
    [CustomEditor(typeof(BaseActionNode))]
    public class ActionListEditor : UnityEditor.Editor
    {
        private ReorderableList list;

        private void OnEnable()
        {
            list = new ReorderableList(serializedObject, serializedObject.FindProperty(BaseActionNode.TasksKey), true,
                true, false, true)
            {
                drawElementCallback = DrawElement,
                onRemoveCallback = DeleteElement,
                drawHeaderCallback = rect => { EditorGUI.LabelField(rect, "Tasks"); }
            };
        }

        private void OnDisable()
        {
            AssetDatabase.SaveAssets();
        }

        private void DeleteElement(ReorderableList reorderableList)
        {
            var element = reorderableList.serializedProperty.GetArrayElementAtIndex(reorderableList.index);
            var action = element.boxedValue as ActionTaskComponent;
            
            Undo.RecordObject(element.serializedObject.targetObject, "Delete Task");
            
            reorderableList.serializedProperty.DeleteArrayElementAtIndex(reorderableList.index);
            reorderableList.serializedProperty.serializedObject.ApplyModifiedProperties();
            
            Undo.DestroyObjectImmediate(action);
            
            EditorUtility.SetDirty(reorderableList.serializedProperty.serializedObject.targetObject);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            list.DoLayoutList();
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            var titleProperty = element.boxedValue.GetFieldValue<string>("title");

            rect.y += 2;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element,
                new GUIContent(titleProperty));

            if (Event.current.type == EventType.Used && rect.Contains(Event.current.mousePosition))
            {
                TryCreateEditorWindow((ActionTaskComponent)element.boxedValue);
            }

            rect.y += EditorGUIUtility.singleLineHeight + 2;
        }

        private void TryCreateEditorWindow(ActionTaskComponent taskComponent)
        {
            var taskType = taskComponent.GetType();

            var editorTypes = TypeCache.GetTypesDerivedFrom<ActionComponentEditorWindow>();

            foreach (var editorType in editorTypes)
            {
                var attributes = editorType.GetCustomAttributes(typeof(WindowInspectorAttribute), false);

                if (attributes.Length == 0)
                {
                    continue;
                }

                foreach (WindowInspectorAttribute attribute in attributes)
                {
                    if (attribute.TargetType == taskType)
                    {
                        var editor = (ActionComponentEditorWindow)EditorWindow.GetWindow(attribute.EditorType);
                        
                        editor.InjectActivation(taskComponent);
                        
                        //Selection.SetActiveObjectWithContext(taskComponent, editor);
                        
                        editor.Show();
                    }
                }
            }
        }
    }
}