using System;
using Module.InteractiveEditor.Configs;
using Module.Utils;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace Module.InteractiveEditor.Editor
{
    [NodeView("Conditions/Conditions", typeof(ConditionsNode))]
    public class ConditionsNodeView : NodeView
    {
        public ConditionsNodeView(BaseNode node) : base(node)
        {
        }

        public override Type InputPortType => typeof(ConditionsNodeView);
        public override Type OutputPortType => typeof(bool);
        public override Port.Capacity InputPortCapacity => Port.Capacity.Multi;
        public override Port.Capacity OutputPortCapacity => Port.Capacity.Multi;
        public override string GetClassTag => "conditions";
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