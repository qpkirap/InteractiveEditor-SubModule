using System;

namespace Module.InteractiveEditor.Editor
{
    public class NodeViewAttribute : Attribute
    {
        public Type BaseNodeType { get; }
        
        public NodeViewAttribute(Type baseNodeType)
        {
            BaseNodeType = baseNodeType;
        }
    }
}