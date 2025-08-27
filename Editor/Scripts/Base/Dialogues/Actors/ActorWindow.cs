using Module.InteractiveEditor.Configs;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Module.InteractiveEditor.Editor
{
    public class ActorWindow : OdinEditorWindow
    {
        [ShowInInspector, HideLabel]
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private Actor currentActor;

        // Removed MenuItem to clean up Interactive Editor menu
        // Use double-click Actor to open
        public static void ShowWindow()
        {
            var window = GetWindow<ActorWindow>();
            window.titleContent = new GUIContent("Actor Editor");
            window.Show();
        }

        public static void ShowWindow(Actor actor)
        {
            var window = GetWindow<ActorWindow>();
            window.titleContent = new GUIContent($"Actor Editor - {actor.Title}");
            window.currentActor = actor;
            window.Show();
        }

        public void InjectActivation(Actor actor)
        {
            currentActor = actor;
            titleContent = new GUIContent($"Actor Editor - {actor.Title}");
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Save Asset", ButtonSizes.Medium)]
        private void SaveAsset()
        {
            if (currentActor != null)
            {
                EditorUtility.SetDirty(currentActor);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Saved", "Actor asset has been saved successfully.", "OK");
            }
        }

        protected override void OnImGUI()
        {
            if (currentActor == null)
            {
                EditorGUILayout.HelpBox("No actor selected. Use InjectActivation to set an actor.", MessageType.Info);
                return;
            }

            base.OnImGUI();
        }

        protected override void OnDestroy()
        {
            if (currentActor != null)
            {
                EditorUtility.SetDirty(currentActor);
            }
            base.OnDestroy();
        }
    }
}