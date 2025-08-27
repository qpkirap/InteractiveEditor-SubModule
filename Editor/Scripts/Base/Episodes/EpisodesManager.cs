using Module.InteractiveEditor.Configs;
using Module.Utils;
using Module.Utils.Configs;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Module.InteractiveEditor.Editor
{
    public class EpisodesManager : OdinEditorWindow
    {
        [ShowInInspector]
        [LabelText("Story Content")]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [PropertySpace(10)]
        private StoryObject currentStoryObject;

        [MenuItem("InteractiveEditor/EpisodesManager")]
        public static void ShowWindow()
        {
            EditorsCache.Init();
            var window = GetWindow<EpisodesManager>();
            window.titleContent = new GUIContent("Episodes Manager");
            window.UpdateCurrentStoryObject();
            window.Show();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCurrentStoryObject();
            Selection.selectionChanged += OnSelectionChanged;
        }

        protected override void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            base.OnDisable();
        }

        private void OnSelectionChanged()
        {
            UpdateCurrentStoryObject();
        }

        private void UpdateCurrentStoryObject()
        {
            var selectedStory = Selection.activeObject as StoryObject;
            var newStoryObject = selectedStory != null ? selectedStory : EditorsCache.CurrentStoryObject;

            if (currentStoryObject != newStoryObject)
            {
                currentStoryObject = newStoryObject;
            }
        }

        [FoldoutGroup("Story Actions")]
        [Button("Add New Episode", ButtonSizes.Medium)]
        private void AddNewEpisode()
        {
            if (currentStoryObject == null)
            {
                EditorUtility.DisplayDialog("Error", "No StoryObject selected. Please select a StoryObject first.", "OK");
                return;
            }

            var instance = ScriptableEntity.Create<EpisodeData>();

            Undo.RecordObject(currentStoryObject, "Add New Episode");

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(instance, currentStoryObject);
            }

            currentStoryObject.AddToList(StoryObject.EpisodeDatasKey, instance);

            Undo.RegisterCreatedObjectUndo(instance, "Add New Episode");
            EditorUtility.SetDirty(currentStoryObject);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Success", "New episode has been created successfully.", "OK");
        }

        [FoldoutGroup("Story Actions")]
        [Button("Add New Actor", ButtonSizes.Medium)]
        private void AddNewActor()
        {
            if (currentStoryObject == null)
            {
                EditorUtility.DisplayDialog("Error", "No StoryObject selected. Please select a StoryObject first.", "OK");
                return;
            }

            var instance = ScriptableEntity.Create<Actor>();

            Undo.RecordObject(currentStoryObject, "Add New Actor");

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(instance, currentStoryObject);
            }

            currentStoryObject.AddToList(StoryObject.ActorsKey, instance);

            Undo.RegisterCreatedObjectUndo(instance, "Add New Actor");
            EditorUtility.SetDirty(currentStoryObject);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Success", "New actor has been created successfully.", "OK");
        }

        [FoldoutGroup("Story Actions")]
        [Button("Save Story Changes", ButtonSizes.Medium)]
        private void SaveStoryChanges()
        {
            if (currentStoryObject != null)
            {
                EditorUtility.SetDirty(currentStoryObject);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Saved", "Story has been saved successfully.", "OK");
            }
        }

        protected override void OnImGUI()
        {
            if (currentStoryObject == null)
            {
                EditorGUILayout.HelpBox("No StoryObject selected. Please select a StoryObject in the Project window to manage its episodes.", MessageType.Info);
                
                if (GUILayout.Button("Try to Find StoryObject"))
                {
                    EditorsCache.UpdateStoryObjects();
                    UpdateCurrentStoryObject();
                }
                return;
            }

            base.OnImGUI();
        }
    }
}