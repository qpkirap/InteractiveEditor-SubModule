using System;
using System.Collections.Generic;
using System.Linq;
using Module.InteractiveEditor.Configs;
using Module.Utils;
using Module.Utils.Configs;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class InteractiveGraphView : GraphView
    {
        private StoryObject currentStory;
        
        public new class UxmlFactory : UxmlFactory<InteractiveGraphView, GraphView.UxmlTraits> { }
        
        public Action<NodeView> OnSelectNode;

        public InteractiveGraphView()
        {
            Insert(0, new GridBackground());
            
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            var styles = AssetDatabase.LoadAssetAtPath<StyleSheet>(Paths.Uss);
            
            styleSheets.Add(styles);

            Undo.undoRedoPerformed = null;
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        public void OnOpen(StoryObject story)
        {
            this.currentStory = story;
            
            graphViewChanged -= OnGraphViewChanged;

            foreach (var graphElement in graphElements)
            {
                if (graphElement is NodeView nodeView)
                {
                    nodeView.OnSelectedNode -= OnSelectNode;
                }
            }
            
            DeleteElements(graphElements);
            
            graphViewChanged += OnGraphViewChanged;
            
            if (currentStory.Nodes == null) return;
            
            //create node
            foreach (var node in currentStory.Nodes)
            {
                GetNodeView(node);
            }
            
            //create edge
            foreach (var parentNode in currentStory.Nodes)
            {
                if (parentNode == null || parentNode.ChildrenNodes == null) continue;
                
                foreach (var childrenNode in parentNode.ChildrenNodes)
                {
                    var parentView = FindNodeView(parentNode);
                    var childView = FindNodeView(childrenNode);
                    
                    var edge = parentView.OutputPort.ConnectTo(childView.InputPort);
                    
                    AddElement(edge);
                }
            }
        }

        public void UpdateState()
        {
            nodes.ForEach(node =>
            {
                if (node is NodeView nodeView)
                {
                    nodeView.UpdateState();
                }
            });
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var types = TypeCache.GetTypesDerivedFrom<NodeView>();

            foreach (var type in types)
            {
                var attributes = type.GetCustomAttributes(typeof(NodeViewAttribute), false);
                
                if (attributes.Length == 0)
                {
                    continue;
                }
                
                foreach (NodeViewAttribute attribute in attributes)
                {
                    if (attribute.BaseNodeType != null && attribute.MenuPath != null)
                    {
                        evt.menu.AppendAction($"{attribute.MenuPath}", a => CreateNode(attribute.BaseNodeType));
                    }
                    else
                    {
                        Debug.LogError($"No BaseNodeType found on {type.Name}");
                    }
                }
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports
                .ToList()
                .Where(endPort => endPort.direction != startPort.direction 
                                  && endPort.node != startPort.node
                                  && endPort.portType == startPort.portType)
                .ToList();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
        {
            if (graphviewchange.elementsToRemove != null)
            {
                graphviewchange.elementsToRemove.ForEach(element =>
                {
                    if (element is NodeView node)
                    {
                        DeleteNode(node.Node);
                    }

                    if (element is Edge edge)
                    {
                        var parentView = edge.output.node as NodeView;
                        var childView = edge.input.node as NodeView;
                        
                        if (parentView != null && childView != null) parentView.RemoveChildNode(childView.Node);
                    }
                });
            }

            if (graphviewchange.edgesToCreate != null)
            {
                graphviewchange.edgesToCreate.ForEach(edge =>
                {
                    var parentView = edge.output.node as NodeView;
                    var childView = edge.input.node as NodeView;

                    if (parentView != null && childView != null) parentView.AddChildNode(childView.Node);
                });
            }

            if (graphviewchange.movedElements != null)
            {
                graphviewchange.movedElements.ForEach(node =>
                {
                    if (node is NodeView nodeView) nodeView.SortingChildren();
                });
            }
            
            return graphviewchange;
        }
        
        private void OnUndoRedoPerformed()
        {
            OnOpen(currentStory);
            
            AssetDatabase.SaveAssets();
        }
        
        private NodeView FindNodeView(BaseNode node)
        {
            return GetNodeByGuid(node.Id) as NodeView;
        }

        private BaseNode CreateNode(Type type)
        {
            var instance = ScriptableEntity.Create<BaseNode>(type);
            
            Undo.RecordObject(currentStory, "Create Node");
            
            currentStory.AddToList(StoryObject.NodesKey, instance);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(instance, currentStory);
            }
            
            Undo.RegisterCreatedObjectUndo(instance, "Create Node");
            
            EditorUtility.SetDirty(currentStory);
            
            AssetDatabase.SaveAssets();
            
            GetNodeView(instance);
            
            return instance;
        }

        private void DeleteNode(BaseNode node)
        {
            Undo.RecordObject(currentStory, "Delete Node");
            
            currentStory.RemoveFromList(StoryObject.NodesKey, node);
            
            //AssetDatabase.RemoveObjectFromAsset(node);
            
            Undo.DestroyObjectImmediate(node);
            
            AssetDatabase.SaveAssets();
        }

        private void GetNodeView(BaseNode node)
        {
            var nodeView = CreateView(node);

            if (nodeView == null)
            {
                Debug.LogError("No NodeView found for " + node.GetType().Name);
                
                return;
            }
            
            nodeView.Init();

            nodeView.title = string.IsNullOrEmpty(node.Title) ? node.GetType().Name : node.Title;
            
            nodeView.OnSelectedNode += OnSelectNode;
            
            AddElement(nodeView);
        }

        private NodeView CreateView(BaseNode node)
        {
            var viewTypes = TypeCache.GetTypesDerivedFrom<NodeView>();

            foreach (var viewType in viewTypes)
            {
                var attributes = viewType.GetCustomAttributes(typeof(NodeViewAttribute), false);

                if (attributes.Length == 0)
                {
                    throw new NotImplementedException("No NodeViewAttribute found on " + viewType.Name);
                    
                    continue;
                }
                
                foreach (NodeViewAttribute attribute in attributes)
                {
                    if (attribute.BaseNodeType == node.GetType())
                    {
                        var instance = (NodeView)Activator.CreateInstance(viewType, args: new object[] { node });

                        return instance;
                    }
                }
            }

            return null;
        }
    }
}