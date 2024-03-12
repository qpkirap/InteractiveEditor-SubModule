using Module.InteractiveEditor.Runtime;
using UnityEditor;

namespace Module.InteractiveEditor.Editor
{
    public abstract class ActionComponentEditorWindow : EditorWindow
    {
        public abstract void InjectActivation(ActionTaskComponent component);
    }
    
    public abstract class ActionComponentEditorWindow<T> : ActionComponentEditorWindow
        where T : ActionTaskComponent
    {
        private SerializedProperty titleProperty;

        protected SerializedObject Container;
        protected T Action { get; private set; }
        
        protected abstract string Title { get; }

        public override void InjectActivation(ActionTaskComponent component)
        {
            Action = (T)component;
            
            Container = new SerializedObject(Action);
            titleProperty = Container.FindProperty("title");
        }

        protected virtual void DrawWindow()
        {
        }

        protected virtual void OnGUI()
        {
            EditorGUILayout.PropertyField(titleProperty);
            
            DrawWindow();
            
            Container.ApplyModifiedProperties();
        }
    }
}