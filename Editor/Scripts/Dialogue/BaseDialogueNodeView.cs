using System;
using Module.InteractiveEditor.Configs;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace Module.InteractiveEditor.Editor
{
    [NodeView(typeof(BaseDialogueNode))]
    public class BaseDialogueNodeView : NodeView
    {
        public BaseDialogueNodeView(BaseNode node) : base(node)
        {
        }

        public override Type InputPortType => typeof(bool);
        public override Type OutputPortType => typeof(bool);
        public override Port.Capacity InputPortCapacity => Port.Capacity.Single;
        public override Port.Capacity OutputPortCapacity => Port.Capacity.Single;
        public override string GetClassTag => "dialogue";

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