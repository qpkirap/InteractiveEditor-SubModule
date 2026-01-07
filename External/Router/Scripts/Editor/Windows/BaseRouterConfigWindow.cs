#if UNITY_EDITOR

using Managers.Router.Config;
using Module.Utils.Editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Submodules.Router.Editor.Windows
{
    /// <summary>
    /// Универсальное окно редактора для BaseRouterConfig.
    /// Работает с любым конфигом, наследующим BaseRouterConfig.
    /// </summary>
    public class BaseRouterConfigWindow : OdinEditorWindow
    {
        [ShowInInspector, HideLabel]
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private BaseRouterConfig currentConfig;

        public static void ShowWindow()
        {
            var window = GetWindow<BaseRouterConfigWindow>();
            window.titleContent = new GUIContent("Router Config Editor");
            window.Show();
        }

        public static void ShowWindow(BaseRouterConfig config)
        {
            var window = GetWindow<BaseRouterConfigWindow>();
            window.titleContent = new GUIContent($"Router Config - {config.name}");
            window.currentConfig = config;
            window.Show();
        }

        public void InjectActivation(BaseRouterConfig config)
        {
            currentConfig = config;
            titleContent = new GUIContent($"Router Config - {config.name}");
        }

        protected override void OnImGUI()
        {
            if (currentConfig == null)
            {
                DrawNoConfigUI();
                return;
            }

            DrawScriptPathsSection();
            EditorGUILayout.Space(10);
            
            base.OnImGUI();
        }

        private void DrawNoConfigUI()
        {
            EditorGUILayout.HelpBox(
                "No config selected.\nSelect an existing config or create a new one.",
                MessageType.Info
            );

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Select BaseRouterConfig from Project", GUILayout.Height(30)))
            {
                SelectExistingConfig();
            }
        }

        private void DrawScriptPathsSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Script Paths", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            var hasAllPaths = !string.IsNullOrEmpty(currentConfig.SceneKeysScriptPath)
                && !string.IsNullOrEmpty(currentConfig.RoutKeysScriptPath)
                && !string.IsNullOrEmpty(currentConfig.RoutArgsScriptPath)
                && !string.IsNullOrEmpty(currentConfig.LoadingsScriptPath);

            if (!hasAllPaths)
            {
                EditorGUILayout.HelpBox(
                    "Some script paths are not configured. Set paths for key generation.",
                    MessageType.Warning
                );
            }

            DrawPathRow("Scene Keys", currentConfig.SceneKeysScriptPath,
                () => SetScriptPath(RouterConfigScriptGenerator.GetSceneKeyScriptData(currentConfig)));

            DrawPathRow("Route Keys", currentConfig.RoutKeysScriptPath,
                () => SetScriptPath(RouterConfigScriptGenerator.GetRoutKeyScriptData(currentConfig)));

            DrawPathRow("Route Args", currentConfig.RoutArgsScriptPath,
                () => SetScriptPath(RouterConfigScriptGenerator.GetRoutArgsKeyScriptData(currentConfig)));

            DrawPathRow("Loading Keys", currentConfig.LoadingsScriptPath,
                () => SetScriptPath(RouterConfigScriptGenerator.GetLoadingKeyScriptData(currentConfig)));

            EditorGUILayout.EndVertical();
        }

        private void DrawPathRow(string label, string currentPath, System.Action onSetPath)
        {
            EditorGUILayout.BeginHorizontal();
            
            var hasPath = !string.IsNullOrEmpty(currentPath);
            var statusIcon = hasPath ? "✓" : "✗";
            var oldColor = GUI.color;
            GUI.color = hasPath ? Color.green : Color.red;
            EditorGUILayout.LabelField(statusIcon, GUILayout.Width(20));
            GUI.color = oldColor;
            
            EditorGUILayout.LabelField(label, GUILayout.Width(100));
            
            if (hasPath)
            {
                EditorGUILayout.LabelField(currentPath, EditorStyles.miniLabel);
            }
            else
            {
                if (GUILayout.Button("Set Path", GUILayout.Width(80)))
                {
                    onSetPath?.Invoke();
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void SetScriptPath(KeyScriptData scriptData)
        {
            var fileName = scriptData.FileName;
            var configPath = AssetDatabase.GetAssetPath(currentConfig);
            var defaultFolder = System.IO.Path.GetDirectoryName(configPath);
            
            var assetPath = EditorUtility.SaveFilePanelInProject(
                $"Save {fileName}.cs",
                fileName,
                "cs",
                $"Choose location for {fileName}.cs",
                defaultFolder
            );

            if (!string.IsNullOrEmpty(assetPath))
            {
                var relativePath = assetPath.Replace("Assets", "");
                scriptData.onPathCreated?.Invoke(relativePath);
                
                KeyScriptFileUtils.CreateScriptFile(assetPath, scriptData);
                EditorUtility.SetDirty(currentConfig);
                AssetDatabase.SaveAssets();
            }
        }

        private void SelectExistingConfig()
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(BaseRouterConfig).Name}");
            
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var config = AssetDatabase.LoadAssetAtPath<BaseRouterConfig>(path);
                InjectActivation(config);
            }
            else
            {
                EditorUtility.DisplayDialog("Not Found", "No BaseRouterConfig found in project.", "OK");
            }
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Save Asset", ButtonSizes.Medium)]
        private void SaveAsset()
        {
            if (currentConfig != null)
            {
                EditorUtility.SetDirty(currentConfig);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Saved", "Config saved.", "OK");
            }
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Ping Asset", ButtonSizes.Medium)]
        private void PingAsset()
        {
            if (currentConfig != null)
            {
                EditorGUIUtility.PingObject(currentConfig);
            }
        }

        [FoldoutGroup("Script Generation")]
        [Button("Update Scene Keys", ButtonSizes.Medium)]
        private void UpdateSceneKeys()
        {
            if (currentConfig != null && ValidatePath(currentConfig.SceneKeysScriptPath, "Scene Keys"))
            {
                RouterConfigScriptGenerator.UpdateSceneScriptFiles(currentConfig);
                EditorUtility.DisplayDialog("Success", "Scene keys updated.", "OK");
            }
        }

        [FoldoutGroup("Script Generation")]
        [Button("Update Route Keys", ButtonSizes.Medium)]
        private void UpdateRouteKeys()
        {
            if (currentConfig != null && ValidatePath(currentConfig.RoutKeysScriptPath, "Route Keys"))
            {
                RouterConfigScriptGenerator.UpdateRoutScriptFiles(currentConfig);
                EditorUtility.DisplayDialog("Success", "Route keys updated.", "OK");
            }
        }

        [FoldoutGroup("Script Generation")]
        [Button("Update Loading Keys", ButtonSizes.Medium)]
        private void UpdateLoadingKeys()
        {
            if (currentConfig != null && ValidatePath(currentConfig.LoadingsScriptPath, "Loading Keys"))
            {
                RouterConfigScriptGenerator.UpdateLoadingScriptFiles(currentConfig);
                EditorUtility.DisplayDialog("Success", "Loading keys updated.", "OK");
            }
        }

        [FoldoutGroup("Script Generation")]
        [Button("Update All Keys", ButtonSizes.Large)]
        private void UpdateAllKeys()
        {
            if (currentConfig != null)
            {
                var allValid = ValidatePath(currentConfig.SceneKeysScriptPath, "Scene Keys")
                    && ValidatePath(currentConfig.RoutKeysScriptPath, "Route Keys")
                    && ValidatePath(currentConfig.LoadingsScriptPath, "Loading Keys");

                if (allValid)
                {
                    RouterConfigScriptGenerator.UpdateAllScriptFiles(currentConfig);
                    EditorUtility.DisplayDialog("Success", "All keys updated.", "OK");
                }
            }
        }

        private bool ValidatePath(string path, string pathName)
        {
            if (string.IsNullOrEmpty(path))
            {
                EditorUtility.DisplayDialog("Path Not Set", 
                    $"{pathName} path is not configured.", "OK");
                return false;
            }
            return true;
        }

        protected override void OnDestroy()
        {
            if (currentConfig != null)
            {
                EditorUtility.SetDirty(currentConfig);
            }
            base.OnDestroy();
        }
    }
}

#endif
