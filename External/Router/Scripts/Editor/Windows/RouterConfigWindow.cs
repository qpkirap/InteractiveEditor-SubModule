#if UNITY_EDITOR

using Managers.Router.Config;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Submodules.Router.Editor.Windows
{
    public class RouterConfigWindow : OdinEditorWindow
    {
        [ShowInInspector, HideLabel]
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private RouterConfig currentRouterConfig;

        public static void ShowWindow()
        {
            var window = GetWindow<RouterConfigWindow>();
            window.titleContent = new GUIContent("Router Config Editor");
            window.Show();
        }

        public static void ShowWindow(RouterConfig routerConfig)
        {
            var window = GetWindow<RouterConfigWindow>();
            window.titleContent = new GUIContent($"Router Config Editor - {routerConfig.name}");
            window.currentRouterConfig = routerConfig;
            window.Show();
        }

        public void InjectActivation(RouterConfig routerConfig)
        {
            currentRouterConfig = routerConfig;
            titleContent = new GUIContent($"Router Config Editor - {routerConfig.name}");
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Save Asset", ButtonSizes.Medium)]
        private void SaveAsset()
        {
            if (currentRouterConfig != null)
            {
                EditorUtility.SetDirty(currentRouterConfig);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Saved", "Router Config asset has been saved successfully.", "OK");
            }
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Ping Asset", ButtonSizes.Medium)]
        private void PingAsset()
        {
            if (currentRouterConfig != null)
            {
                EditorGUIUtility.PingObject(currentRouterConfig);
            }
        }
        
        [FoldoutGroup("Script Generation")]
        [Button("Update Scene Keys", ButtonSizes.Medium)]
        private void UpdateSceneKeys()
        {
            if (currentRouterConfig != null)
            {
                RouterConfigScriptGenerator.UpdateSceneScriptFiles(currentRouterConfig);
                EditorUtility.DisplayDialog("Success", "Scene keys script file has been updated.", "OK");
            }
        }
        
        [FoldoutGroup("Script Generation")]
        [Button("Update Route Keys", ButtonSizes.Medium)]
        private void UpdateRouteKeys()
        {
            if (currentRouterConfig != null)
            {
                RouterConfigScriptGenerator.UpdateRoutScriptFiles(currentRouterConfig);
                EditorUtility.DisplayDialog("Success", "Route keys script files have been updated.", "OK");
            }
        }
        
        [FoldoutGroup("Script Generation")]
        [Button("Update Loading Keys", ButtonSizes.Medium)]
        private void UpdateLoadingKeys()
        {
            if (currentRouterConfig != null)
            {
                RouterConfigScriptGenerator.UpdateLoadingScriptFiles(currentRouterConfig);
                EditorUtility.DisplayDialog("Success", "Loading keys script file has been updated.", "OK");
            }
        }
        
        [FoldoutGroup("Script Generation")]
        [Button("Update All Keys", ButtonSizes.Large)]
        private void UpdateAllKeys()
        {
            if (currentRouterConfig != null)
            {
                RouterConfigScriptGenerator.UpdateSceneScriptFiles(currentRouterConfig);
                RouterConfigScriptGenerator.UpdateRoutScriptFiles(currentRouterConfig);
                RouterConfigScriptGenerator.UpdateLoadingScriptFiles(currentRouterConfig);
                EditorUtility.DisplayDialog("Success", "All key script files have been updated.", "OK");
            }
        }

        protected override void OnImGUI()
        {
            if (currentRouterConfig == null)
            {
                EditorGUILayout.HelpBox("No Router Config selected. Use InjectActivation to set a router config or select one from the project.", MessageType.Info);
                
                if (GUILayout.Button("Select Router Config from Project"))
                {
                    var configs = AssetDatabase.FindAssets($"t:{typeof(RouterConfig).Name}");
                    if (configs.Length > 0)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(configs[0]);
                        var config = AssetDatabase.LoadAssetAtPath<RouterConfig>(path);
                        InjectActivation(config);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("No Router Config Found", "No Router Config assets found in the project.", "OK");
                    }
                }
                return;
            }

            base.OnImGUI();
        }

        protected override void OnDestroy()
        {
            if (currentRouterConfig != null)
            {
                EditorUtility.SetDirty(currentRouterConfig);
            }
            base.OnDestroy();
        }
    }
}

#endif