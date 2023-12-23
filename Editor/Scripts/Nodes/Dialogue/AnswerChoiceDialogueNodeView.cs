using System;
using Module.InteractiveEditor.Configs;
using Module.Utils;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace Module.InteractiveEditor.Editor
{
    [NodeView("Dialogue/Choices/AnswerChoice", typeof(AnswerChoiceDialogueNode))]
    public class AnswerChoiceDialogueNodeView : NodeView
    {
        public AnswerChoiceDialogueNodeView(BaseNode node) : base(node)
        {
        }

        public override Type InputPortType => typeof(AnswerChoiceDialogueNodeView);
        public override Type OutputPortType { get; }
        public override Port.Capacity InputPortCapacity => Port.Capacity.Single;
        public override Port.Capacity OutputPortCapacity => Port.Capacity.Single;
        public override string GetClassTag => "dialogue";
        public override void AddChildNode(BaseNode node)
        {
            Undo.RecordObject(Node, "Add from Answer Node");
            
            Node.AddToList(BaseNode.ChildNodeKey, node);
            
            EditorUtility.SetDirty(Node);
        }

        public override void RemoveChildNode(BaseNode node)
        {
            Undo.RecordObject(Node, "Remove from Answer Node");
         
            Node.RemoveFromList(BaseNode.ChildNodeKey, node);
            
            EditorUtility.SetDirty(Node);
        }

        public override void Visit(INodeVisitor visitor)
        {
        }
    }
}