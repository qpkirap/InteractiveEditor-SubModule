using System;

namespace Module.InteractiveEditor.Editor
{
    public class ToolbarComponentAttribute : Attribute
    {
        public readonly Type ActionType;
        public readonly Type[] ContainerFilter;
        
        public ToolbarComponentAttribute(Type actionType, params Type[] containerFilter)
        {
            ActionType = actionType;
            ContainerFilter = containerFilter;
        }
    }
}