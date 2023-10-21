using UnityEngine;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class InspectorView : VisualElement
    {
        private UnityEditor.Editor editor;
        
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

        public void UpdateSelection(NodeView nodeView)
        {
            Clear();
            
            Object.DestroyImmediate(editor);
            
            if (nodeView == null) return;

            editor = UnityEditor.Editor.CreateEditor(nodeView.Node);

            var imgContainer = new IMGUIContainer(() =>
            {
                if (editor.target != null) editor.OnInspectorGUI();
            });
            
            Add(imgContainer);
        }
    }
}