using System;
using System.Linq;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Runtime;
using Module.Utils;
using Module.Utils.Configs;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Module.InteractiveEditor.Editor
{
    [NodeView(typeof(BaseActionNode))]
    public class BaseActionNodeView : NodeView
    {
        public BaseActionNodeView(BaseNode node) : base(node)
        {
            InitToolbar();
        }

        public override Type InputPortType => typeof(bool);
        public override Type OutputPortType => typeof(bool);
        public override Port.Capacity InputPortCapacity => Port.Capacity.Single;
        public override Port.Capacity OutputPortCapacity => Port.Capacity.Single;
        public override string GetClassTag => "action";
        
        private void InitToolbar()
        {
            toolbarItems.Add("test", () => Debug.Log("test"));

            var actionsTypes = TypeCache.GetTypesDerivedFrom<ActionTaskComponent>();
            
            foreach (var actionType in actionsTypes)
            {
                var attributes = actionType.GetCustomAttributes(typeof(ToolbarComponentAttribute), false);
                
                if (attributes.Length == 0)
                {
                    continue;
                }
                
                foreach (ToolbarComponentAttribute attribute in attributes)
                {
                    if (attribute.ContainerFilter.Contains(typeof(ActionTaskComponent)))
                    {
                        toolbarItems.Add(attribute.ActionType.Name, () => CreateTask(attribute.ActionType));
                    }
                }
            }
        }
        
        private ActionTaskComponent CreateTask(Type type)
        {
            var instance = ScriptableEntity.Create<ActionTaskComponent>(type);
            
            Undo.RecordObject(Node, "Create Action");
            
            Node.AddToList(BaseActionNode.TasksKey, instance);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(instance, Node);
            }
            
            Undo.RegisterCreatedObjectUndo(instance, "Create Action");
            
            EditorUtility.SetDirty(Node);
            
            AssetDatabase.SaveAssets();
            
            return instance;
        }
        
        public override void AddChildNode(BaseNode node)
        {
            Undo.RecordObject(Node, "Add Child Node");
            
            Node.AddToList(BaseNode.ChildNodeKey, node);
            
            EditorUtility.SetDirty(Node);
        }

        public override void RemoveChildNode(BaseNode node)
        {
            Undo.RecordObject(Node, "Remove Child Node");
         
            Node.RemoveFromList(BaseNode.ChildNodeKey, node);
            
            EditorUtility.SetDirty(Node);
        }
    }
}