#if UNITY_EDITOR

using Managers.Router.Config;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Submodules.Router.Editor.Windows
{
    public class RoutDataWindow : OdinEditorWindow
    {
        [ShowInInspector, HideLabel]
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private RoutData currentRoutData;

        public static void ShowWindow()
        {
            var window = GetWindow<RoutDataWindow>();
            window.titleContent = new GUIContent("Rout Data Editor");
            window.Show();
        }

        public static void ShowWindow(RoutData routData)
        {
            var window = GetWindow<RoutDataWindow>();
            window.titleContent = new GUIContent($"Rout Data Editor - {routData.Title}");
            window.currentRoutData = routData;
            window.Show();
        }

        public void InjectActivation(RoutData routData)
        {
            currentRoutData = routData;
            titleContent = new GUIContent($"Rout Data Editor - {routData.Title}");
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Save Asset", ButtonSizes.Medium)]
        private void SaveAsset()
        {
            if (currentRoutData != null)
            {
                EditorUtility.SetDirty(currentRoutData);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Saved", "Rout Data asset has been saved successfully.", "OK");
            }
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Ping Asset", ButtonSizes.Medium)]
        private void PingAsset()
        {
            if (currentRoutData != null)
            {
                EditorGUIUtility.PingObject(currentRoutData);
            }
        }

        protected override void OnImGUI()
        {
            if (currentRoutData == null)
            {
                EditorGUILayout.HelpBox("No Rout Data selected. Use InjectActivation to set a rout data.", MessageType.Info);
                return;
            }

            base.OnImGUI();
        }

        protected override void OnDestroy()
        {
            if (currentRoutData != null)
            {
                EditorUtility.SetDirty(currentRoutData);
            }
            base.OnDestroy();
        }
    }
}

#endif