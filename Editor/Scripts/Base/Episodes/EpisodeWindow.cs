using Module.InteractiveEditor.Configs;
using Module.Utils;
using Module.Utils.Configs;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Module.InteractiveEditor.Editor
{
    public class EpisodeWindow : OdinEditorWindow
    {
        [ShowInInspector, HideLabel]
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private EpisodeData currentEpisode;

        // Removed MenuItem to clean up Interactive Editor menu
        // Use EpisodesManager or double-click EpisodeData to open
        public static void ShowWindow()
        {
            var window = GetWindow<EpisodeWindow>();
            window.titleContent = new GUIContent("Episode Editor");
            window.Show();
        }

        public static void ShowWindow(EpisodeData episode)
        {
            var window = GetWindow<EpisodeWindow>();
            window.titleContent = new GUIContent($"Episode Editor - {episode.Title}");
            window.currentEpisode = episode;
            window.Show();
        }

        public void InjectActivation(EpisodeData episode)
        {
            currentEpisode = episode;
            titleContent = new GUIContent($"Episode Editor - {episode.Title}");
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Open Images Manager", ButtonSizes.Medium)]
        private void OpenImagesManager()
        {
            if (currentEpisode != null)
            {
                ImageWindow.ShowWindow(currentEpisode);
            }
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Add New Image", ButtonSizes.Medium)]
        private void AddNewImage()
        {
            if (currentEpisode != null)
            {
                var newImage = ScriptableEntity.Create<ImageData>();
                
                Undo.RecordObject(currentEpisode, "Add New Image");
                
                if (!Application.isPlaying)
                {
                    AssetDatabase.AddObjectToAsset(newImage, currentEpisode);
                }
                
                currentEpisode.AddToList(EpisodeData.ImageDatasKey, newImage);
                
                Undo.RegisterCreatedObjectUndo(newImage, "Add New Image");
                EditorUtility.SetDirty(currentEpisode);
                AssetDatabase.SaveAssets();
                
                // Open the new image in the image editor
                ImageWindow.ShowWindow(newImage);
            }
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Save Asset", ButtonSizes.Medium)]
        private void SaveAsset()
        {
            if (currentEpisode != null)
            {
                EditorUtility.SetDirty(currentEpisode);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Saved", "Episode asset has been saved successfully.", "OK");
            }
        }

        protected override void OnImGUI()
        {
            if (currentEpisode == null)
            {
                EditorGUILayout.HelpBox("No episode selected. Use InjectActivation to set an episode or use the Episodes Window.", MessageType.Info);
                
                if (GUILayout.Button("Open Episodes Manager"))
                {
                    EpisodesManager.ShowWindow();
                }
                return;
            }

            base.OnImGUI();
        }

        protected override void OnDestroy()
        {
            if (currentEpisode != null)
            {
                EditorUtility.SetDirty(currentEpisode);
            }
            base.OnDestroy();
        }
    }
}