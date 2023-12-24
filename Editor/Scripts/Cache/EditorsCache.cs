using System.Collections.Generic;
using Module.InteractiveEditor.Configs;
using UnityEditor;

namespace Module.InteractiveEditor.Editor
{
    public static class EditorsCache
    {
        private static StoryObject currentStoryObject;
        private static List<StoryObject> storyObjectsAll;
        
        public static IReadOnlyList<StoryObject> StoryObjectsAll => storyObjectsAll;
        public static StoryObject CurrentStoryObject => currentStoryObject;
        
        public static void Init()
        {
            UpdateStoryObjects();
        }
        
        public static void SetCurrentStoryObject(StoryObject storyObject)
        {
            currentStoryObject = storyObject;
        }

        public static void UpdateStoryObjects()
        {
            storyObjectsAll = new List<StoryObject>();
            
            var guids = AssetDatabase.FindAssets($"t:{nameof(StoryObject)}");
            
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                
                var storyObject = AssetDatabase.LoadAssetAtPath<StoryObject>(path);
                
                if (storyObject)
                {
                    storyObjectsAll.Add(storyObject);
                }
            }
        }
    }
}