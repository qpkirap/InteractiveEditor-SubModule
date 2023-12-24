using System.Collections.Generic;
using System.Linq;
using Module.InteractiveEditor.Configs;
using Module.Utils;
using UnityEditor;
using UnityEngine;

namespace Module.InteractiveEditor.Editor
{
    [CustomEditor(typeof(ActorDialogueNode))]
    public class ActorSelectedEditor : BaseDialogueEditor
    {
        private StoryObject storyObject;
        private List<Actor> actors;
        private string[] actorTitles;
        private int selectIndex;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            storyObject = EditorsCache.CurrentStoryObject;

            actors = storyObject != null ? storyObject.GetFieldValue<List<Actor>>(StoryObject.ActorsKey) : new(1);
            
            actorTitles = actors.Select(x => x.Title).ToArray();

            var currentActor = target.GetFieldValue<Actor>(ActorDialogueNode.ActorKey);

            if (currentActor != null)
            {
                selectIndex = actors.IndexOf(currentActor);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (storyObject != null)
            {
                selectIndex = EditorGUILayout.Popup("SelectActor", selectIndex, actorTitles, EditorStyles.popup);
                
                target.SetFieldValue(ActorDialogueNode.ActorKey, actors[selectIndex]);
                
                EditorUtility.SetDirty(target);
                
                if (GUI.changed)
                {
                    AssetDatabase.SaveAssets();
                }
            }
        }
    }
}