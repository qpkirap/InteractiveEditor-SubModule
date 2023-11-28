﻿using System;

namespace Module.InteractiveEditor.Editor
{
    public class ComponentEditorAttribute : Attribute
    {
        public readonly Type ActionType;
        public Type[] ContainerFilter;
        
        public ComponentEditorAttribute(Type actionType, params Type[] containerFilter)
        {
            ActionType = actionType;
            ContainerFilter = containerFilter;
        }
    }
}