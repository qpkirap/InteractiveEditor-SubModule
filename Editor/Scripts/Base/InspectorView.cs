using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class InspectorView : VisualElement
    {
        private UnityEditor.Editor currentEditor;
        private SerializedObject container;
        private SerializedProperty titleProperty;

        private INodeVisitor visitor;
        
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

        public UnityEditor.Editor GetCurrentEditor => currentEditor;

        public void UpdateSelection(NodeView nodeView)
        {
            visitor ??= new InspectorNodeVisitor(this);
            
            Clear();
            
            Object.DestroyImmediate(currentEditor);
            
            var scrollView = new ScrollView();
            Add(scrollView);
            
            if (nodeView == null) return;

            container = new(nodeView.Node);
            var id = container.FindProperty("id").stringValue;
            titleProperty = container.FindProperty("title");

            currentEditor = UnityEditor.Editor.CreateEditor(nodeView.Node);

            var imgContainer = new IMGUIContainer(() =>
            {
                if (currentEditor.target != null)
                {
                    EditorGUILayout.TextField(id);
                    EditorGUILayout.PropertyField(titleProperty);
                    
                    currentEditor.OnInspectorGUI();
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        container.ApplyModifiedProperties();
                    }
                }
            });
            
            scrollView.Add(imgContainer);
            
            visitor.OnSelectNode(nodeView);
        }
    }
}