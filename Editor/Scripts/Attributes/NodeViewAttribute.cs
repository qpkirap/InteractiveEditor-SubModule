using System;

namespace Module.InteractiveEditor.Editor
{
    public class NodeViewAttribute : Attribute
    {
        public Type BaseNodeType { get; }
        public string MenuPath { get;}
        
        public NodeViewAttribute(string menuPath, Type baseNodeType)
        {
            BaseNodeType = baseNodeType;
            MenuPath = menuPath;
        }
    }
}