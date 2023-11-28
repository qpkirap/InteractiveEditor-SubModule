using System;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Runtime;
using Module.Utils;
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

            var actions = TypeCache.GetTypesDerivedFrom<ActionTask>();
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