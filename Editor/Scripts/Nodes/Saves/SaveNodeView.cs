using System;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Runtime;
using Module.Utils;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace Module.InteractiveEditor.Editor
{
    [NodeView("Save/SaveNode", typeof(SaveNode))]
    public class SaveNodeView : NodeView
    {
        public SaveNodeView(BaseNode node) : base(node)
        {
        }

        public override Type InputPortType => typeof(bool);
        public override Type OutputPortType => typeof(bool);
        public override Port.Capacity InputPortCapacity => Port.Capacity.Multi;
        public override Port.Capacity OutputPortCapacity => Port.Capacity.Multi;
        public override string GetClassTag => "save";
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

        public override void Visit(INodeVisitor visitor)
        {
        }
    }
}