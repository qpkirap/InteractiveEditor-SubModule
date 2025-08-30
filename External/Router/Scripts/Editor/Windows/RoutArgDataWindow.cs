#if UNITY_EDITOR

using Managers.Router.Config;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Submodules.Router.Editor.Windows
{
    public class RoutArgDataWindow : OdinEditorWindow
    {
        [ShowInInspector, HideLabel]
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private RoutArgData currentRoutArgData;

        public static void ShowWindow()
        {
            var window = GetWindow<RoutArgDataWindow>();
            window.titleContent = new GUIContent("Rout Arg Data Editor");
            window.Show();
        }

        public static void ShowWindow(RoutArgData routArgData)
        {
            var window = GetWindow<RoutArgDataWindow>();
            window.titleContent = new GUIContent($"Rout Arg Data Editor - {routArgData.Title}");
            window.currentRoutArgData = routArgData;
            window.Show();
        }

        public void InjectActivation(RoutArgData routArgData)
        {
            currentRoutArgData = routArgData;
            titleContent = new GUIContent($"Rout Arg Data Editor - {routArgData.Title}");
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Save Asset", ButtonSizes.Medium)]
        private void SaveAsset()
        {
            if (currentRoutArgData != null)
            {
                EditorUtility.SetDirty(currentRoutArgData);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Saved", "Rout Arg Data asset has been saved successfully.", "OK");
            }
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Ping Asset", ButtonSizes.Medium)]
        private void PingAsset()
        {
            if (currentRoutArgData != null)
            {
                EditorGUIUtility.PingObject(currentRoutArgData);
            }
        }

        protected override void OnImGUI()
        {
            if (currentRoutArgData == null)
            {
                EditorGUILayout.HelpBox("No Rout Arg Data selected. Use InjectActivation to set a rout arg data.", MessageType.Info);
                return;
            }

            base.OnImGUI();
        }

        protected override void OnDestroy()
        {
            if (currentRoutArgData != null)
            {
                EditorUtility.SetDirty(currentRoutArgData);
            }
            base.OnDestroy();
        }
    }
}

#endif