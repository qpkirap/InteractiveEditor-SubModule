using System;

namespace Module.InteractiveEditor.Editor
{
    public class ToolbarComponentAttribute : Attribute
    {
        public readonly string Name;
        public readonly Type ActionType;
        public readonly Type[] ContainerFilter;
        
        public ToolbarComponentAttribute(string name, Type actionType, params Type[] containerFilter)
        {
            Name = name;
            ActionType = actionType;
            ContainerFilter = containerFilter;
        }
    }
}