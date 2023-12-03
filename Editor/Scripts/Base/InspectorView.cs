using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class InspectorView : VisualElement
    {
        private UnityEditor.Editor editor;
        private SerializedObject container;
        private SerializedProperty titleProperty;
        
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

        public void UpdateSelection(NodeView nodeView)
        {
            Clear();
            
            Object.DestroyImmediate(editor);
            
            if (nodeView == null) return;

            container = new(nodeView.Node);
            var id = container.FindProperty("id").stringValue;
            titleProperty = container.FindProperty("title");

            editor = UnityEditor.Editor.CreateEditor(nodeView.Node);

            var imgContainer = new IMGUIContainer(() =>
            {
                if (editor.target != null)
                {
                    EditorGUILayout.TextField(id);
                    EditorGUILayout.PropertyField(titleProperty);
                    
                    editor.OnInspectorGUI();
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        container.ApplyModifiedProperties();
                    }
                }
            });
            
            Add(imgContainer);
        }
    }
}