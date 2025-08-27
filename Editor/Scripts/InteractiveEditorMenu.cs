using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Module.InteractiveEditor.Editor
{
    /// <summary>
    /// Centralized menu for accessing the EpisodesManager
    /// </summary>
    public static class InteractiveEditorMenu
    {
        // Double-click handlers for automatic editor opening (high priority)
        // Priority 0 ensures this handles EpisodeData, ImageData, and Actor before InteractiveEditor
        [OnOpenAsset(0)] // Priority 0 = highest priority
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceId);
            
            // Handle EpisodeData double-click
            if (asset is Module.InteractiveEditor.Configs.EpisodeData episode)
            {
                EpisodeWindow.ShowWindow(episode);
                return true;
            }
            
            // Handle ImageData double-click
            if (asset is Module.InteractiveEditor.Configs.ImageData imageData)
            {
                ImageWindow.ShowWindow(imageData);
                return true;
            }
            
            // Handle Actor double-click
            if (asset is Module.InteractiveEditor.Configs.Actor actor)
            {
                ActorWindow.ShowWindow(actor);
                return true;
            }
            
            return false; // Let Unity handle other asset types
        }

        [MenuItem("InteractiveEditor/EpisodesManager", priority = 10)]
        public static void OpenEpisodesManager()
        {
            EpisodesManager.ShowWindow();
        }
    }
}