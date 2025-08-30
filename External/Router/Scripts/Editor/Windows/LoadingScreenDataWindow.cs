#if UNITY_EDITOR

using Managers.Router.Config.Loading;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Submodules.Router.Editor.Windows
{
    public class LoadingScreenDataWindow : OdinEditorWindow
    {
        [ShowInInspector, HideLabel]
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private LoadingScreenData currentLoadingScreenData;

        public static void ShowWindow()
        {
            var window = GetWindow<LoadingScreenDataWindow>();
            window.titleContent = new GUIContent("Loading Screen Data Editor");
            window.Show();
        }

        public static void ShowWindow(LoadingScreenData loadingScreenData)
        {
            var window = GetWindow<LoadingScreenDataWindow>();
            window.titleContent = new GUIContent($"Loading Screen Data Editor - {loadingScreenData.Title}");
            window.currentLoadingScreenData = loadingScreenData;
            window.Show();
        }

        public void InjectActivation(LoadingScreenData loadingScreenData)
        {
            currentLoadingScreenData = loadingScreenData;
            titleContent = new GUIContent($"Loading Screen Data Editor - {loadingScreenData.Title}");
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Save Asset", ButtonSizes.Medium)]
        private void SaveAsset()
        {
            if (currentLoadingScreenData != null)
            {
                EditorUtility.SetDirty(currentLoadingScreenData);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Saved", "Loading Screen Data asset has been saved successfully.", "OK");
            }
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Ping Asset", ButtonSizes.Medium)]
        private void PingAsset()
        {
            if (currentLoadingScreenData != null)
            {
                EditorGUIUtility.PingObject(currentLoadingScreenData);
            }
        }

        protected override void OnImGUI()
        {
            if (currentLoadingScreenData == null)
            {
                EditorGUILayout.HelpBox("No Loading Screen Data selected. Use InjectActivation to set a loading screen data.", MessageType.Info);
                return;
            }

            base.OnImGUI();
        }

        protected override void OnDestroy()
        {
            if (currentLoadingScreenData != null)
            {
                EditorUtility.SetDirty(currentLoadingScreenData);
            }
            base.OnDestroy();
        }
    }
}

#endif