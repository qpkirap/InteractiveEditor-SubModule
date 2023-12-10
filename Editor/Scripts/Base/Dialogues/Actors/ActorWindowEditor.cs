using Module.InteractiveEditor.Configs;
using UnityEditor;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class ActorWindowEditor : EditorWindow
    {
        private SerializedObject container;
        private SerializedProperty titleProperty;
        private UnityEditor.Editor editor;
        private IMGUIContainer IMGUIContainer;

        public void InjectActivation(Actor actor)
        {
            IMGUIContainer?.Clear();
            
            DestroyImmediate(editor);

            editor = UnityEditor.Editor.CreateEditor(actor);
            
            container = new(actor);
            titleProperty = container.FindProperty("title");

            IMGUIContainer = new IMGUIContainer(() =>
            {
                EditorGUILayout.PropertyField(titleProperty);
                
                editor.OnInspectorGUI();
                
                if (EditorGUI.EndChangeCheck())
                {
                    container.ApplyModifiedProperties();
                }
            });
            
            rootVisualElement.Add(IMGUIContainer);
        }
    }
}