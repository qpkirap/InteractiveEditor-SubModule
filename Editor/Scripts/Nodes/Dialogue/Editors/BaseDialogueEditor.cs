using System;
using Module.InteractiveEditor.Configs;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Module.InteractiveEditor.Editor
{
    [CustomEditor(typeof(BaseDialogueNode))]
    public class BaseDialogueEditor : UnityEditor.Editor
    {
        private ReorderableList list;
        
        public Action OnUpdate { get; internal set; }
        
        protected virtual void OnEnable()
        {
            if (EditorApplication.isPlaying || EditorApplication.isUpdating) return;
            
            list = new ReorderableList(serializedObject, serializedObject.FindProperty(BaseDialogueNode.ImagesKey), 
                true,
                true,
                true,
                true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(rect, "Images"); },
                
                onAddCallback = (_) => OnAdd(),
                onRemoveCallback = (_) => OnRemove(),
                
                onChangedCallback = (_) => OnUpdate?.Invoke(),
                onSelectCallback = (_) => OnUpdate?.Invoke(),
                onMouseUpCallback = (_) => OnUpdate?.Invoke(),
                
                drawElementCallback = DrawElement,
            };
        }

        private void DrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            
            EditorGUI.PropertyField(rect, element, new GUIContent());
            
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        private void OnAdd()
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            
            EditorUtility.SetDirty(list.serializedProperty.serializedObject.targetObject);
            list.serializedProperty.serializedObject.ApplyModifiedProperties();
            
            OnUpdate?.Invoke();
        }

        private void OnRemove()
        {
            list.serializedProperty.DeleteArrayElementAtIndex(list.index);
            
            EditorUtility.SetDirty(list.serializedProperty.serializedObject.targetObject);
            list.serializedProperty.serializedObject.ApplyModifiedProperties();
            
            OnUpdate?.Invoke();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            GUILayout.Space(5);
            
            GUILayout.Label("Select Images", EditorStyles.boldLabel);
            
            list.DoLayoutList();
    
            serializedObject.Update();
            
            var isHasModifiedProperties = serializedObject.hasModifiedProperties;
            
            if (!isHasModifiedProperties) return;
            EditorUtility.SetDirty(serializedObject.targetObject);
                
            serializedObject.ApplyModifiedProperties();
        }
    }
}