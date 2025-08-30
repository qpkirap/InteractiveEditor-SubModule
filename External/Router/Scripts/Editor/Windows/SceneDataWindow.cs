#if UNITY_EDITOR

using Managers.Router.Config;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Submodules.Router.Editor.Windows
{
    public class SceneDataWindow : OdinEditorWindow
    {
        [ShowInInspector, HideLabel]
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private SceneData currentSceneData;

        public static void ShowWindow()
        {
            var window = GetWindow<SceneDataWindow>();
            window.titleContent = new GUIContent("Scene Data Editor");
            window.Show();
        }

        public static void ShowWindow(SceneData sceneData)
        {
            var window = GetWindow<SceneDataWindow>();
            window.titleContent = new GUIContent($"Scene Data Editor - {sceneData.Title}");
            window.currentSceneData = sceneData;
            window.Show();
        }

        public void InjectActivation(SceneData sceneData)
        {
            currentSceneData = sceneData;
            titleContent = new GUIContent($"Scene Data Editor - {sceneData.Title}");
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Save Asset", ButtonSizes.Medium)]
        private void SaveAsset()
        {
            if (currentSceneData != null)
            {
                EditorUtility.SetDirty(currentSceneData);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Saved", "Scene Data asset has been saved successfully.", "OK");
            }
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Ping Asset", ButtonSizes.Medium)]
        private void PingAsset()
        {
            if (currentSceneData != null)
            {
                EditorGUIUtility.PingObject(currentSceneData);
            }
        }

        protected override void OnImGUI()
        {
            if (currentSceneData == null)
            {
                EditorGUILayout.HelpBox("No Scene Data selected. Use InjectActivation to set a scene data.", MessageType.Info);
                return;
            }

            base.OnImGUI();
        }

        protected override void OnDestroy()
        {
            if (currentSceneData != null)
            {
                EditorUtility.SetDirty(currentSceneData);
            }
            base.OnDestroy();
        }
    }
}

#endif