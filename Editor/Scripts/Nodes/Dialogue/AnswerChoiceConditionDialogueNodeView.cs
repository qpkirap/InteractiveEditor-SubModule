using System;
using Module.InteractiveEditor.Configs;
using Module.Utils;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace Module.InteractiveEditor.Editor
{
    [NodeView("Dialogue/Choices/AnswerChoiceConditions", typeof(AnswerChoiceDialogueNode))]
    public class AnswerChoiceConditionDialogueNodeView : NodeView
    {
        public AnswerChoiceConditionDialogueNodeView(BaseNode node) : base(node)
        {
        }

        public override Type InputPortType => typeof(AnswerChoiceConditionDialogueNodeView);
        public override Type OutputPortType => typeof(bool);
        public override Port.Capacity InputPortCapacity => Port.Capacity.Multi;
        public override Port.Capacity OutputPortCapacity => Port.Capacity.Multi;
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