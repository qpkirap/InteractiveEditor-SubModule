﻿﻿﻿﻿﻿using System.Linq;
using Module.InteractiveEditor.Runtime;
using Module.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    public class ActorDialogueNode : BaseDialogueNode<ActorDialogueExecutor>
    {
        [FoldoutGroup("Actor Settings")]
        [ValueDropdown(nameof(GetAvailableActors))]
        [LabelText("Select Actor")]
        [SerializeField] private Actor actor;
        
        public Actor Actor => actor;

        public const string ActorKey = nameof(actor);
        
        private ValueDropdownList<Actor> GetAvailableActors()
        {
            var dropdown = new ValueDropdownList<Actor>();
            
            // Try to find the parent StoryObject
            var storyObject = FindParentStoryObject();
            if (storyObject == null)
            {
                dropdown.Add("No StoryObject found", null);
                return dropdown;
            }
            
            var actors = storyObject.Actors;
            if (actors == null || actors.Count == 0)
            {
                dropdown.Add("No actors available", null);
                return dropdown;
            }
            
            foreach (var actorItem in actors)
            {
                if (actorItem != null)
                {
                    var displayName = !string.IsNullOrEmpty(actorItem.Title) ? actorItem.Title : actorItem.name;
                    dropdown.Add(displayName, actorItem);
                }
            }
            
            return dropdown;
        }
        
        public override object Clone()
        {
            var item =  base.Clone();
            
            item.SetFieldValue(ActorKey, actor);
            
            return item;
        }
    }
}