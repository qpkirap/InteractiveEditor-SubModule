using System;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class InteractiveEditor : EditorWindow
    {
        private InteractiveGraphView graphView;
        private InspectorView inspectorView;
        private ToolbarMenu toolbarMenu;
        
        [MenuItem("InteractiveEditor/Editor")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<InteractiveEditor>();
            
            wnd.titleContent = new GUIContent("InteractiveEditor");
        }

        public void CreateGUI()
        {
            if (graphView != null) graphView.OnSelectNode -= OnSelectNode;
            
            var root = rootVisualElement;
        
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Paths.Uxml);
            var styles = AssetDatabase.LoadAssetAtPath<StyleSheet>(Paths.Uss);

            uxml.CloneTree(root);
            
            root.styleSheets.Add(styles);

            graphView = root.Q<InteractiveGraphView>();
            inspectorView = root.Q<InspectorView>();
            toolbarMenu = root.Q<ToolbarMenu>();
            
            graphView.OnSelectNode += OnSelectNode;
            
            OnSelectionChange();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnDestroy()
        {
            if (graphView != null) graphView.OnSelectNode -= OnSelectNode;
        }
        
        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
            }
        }

        private void OnSelectNode(NodeView nodeView)
        {
            inspectorView.UpdateSelection(nodeView);
            toolbarMenu.menu.ClearItems();
            
            foreach (var kv in nodeView.ToolbarItems)
            {
                toolbarMenu.menu.AppendAction(kv.Key, _ => kv.Value?.Invoke());
            }
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject != null && Selection.activeObject is StoryObject entity)
            {
                OpenWindow();

                return true;
            }

            return false;
        }

        private void OnSelectionChange()
        {
            var story = Selection.activeObject as StoryObject;
            
            if (!story)
            {
                if (Selection.activeGameObject)
                {
                    var controller = Selection.activeGameObject.GetComponent<StoryObjectController>();

                    if (controller)
                    {
                        story = controller.StoryObject;
                    }
                }
            }

            if (Application.isPlaying)
            {
                if (story)
                {
                    graphView?.OnOpen(story);
                }
            }
            else if(story && AssetDatabase.CanOpenAssetInEditor(story.GetInstanceID()))
            {
                graphView?.OnOpen(story);
            }
        }

        private void OnInspectorUpdate()
        {
            graphView?.UpdateState();
        }
    }
}