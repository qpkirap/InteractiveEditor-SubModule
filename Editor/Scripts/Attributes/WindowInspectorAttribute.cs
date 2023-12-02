using System;

namespace Module.InteractiveEditor.Editor
{
    public class WindowInspectorAttribute : Attribute
    {
        public Type TargetType { get; }
        public Type EditorType { get; }
        
        public WindowInspectorAttribute(Type targetType, Type editorType)
        {
            TargetType = targetType;
            EditorType = editorType;
        }
    }
}