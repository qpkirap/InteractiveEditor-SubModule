using System;
using Module.InteractiveEditor.Configs;
using UnityEditor.Experimental.GraphView;

namespace Module.InteractiveEditor.Editor
{
    [NodeView("Dialogue/Choices/Save/SelectChoiceDialogue",typeof(SelectChoiceDialogueConditionNode))]
    public class SelectChoiceDialogueConditionNodeView : SelectChoiceDialogueNodeView
    {
        public SelectChoiceDialogueConditionNodeView(BaseNode node) : base(node)
        {
        }
        
        public override Port.Capacity InputPortCapacity => Port.Capacity.Multi;
        public override Port.Capacity OutputPortCapacity => Port.Capacity.Multi;

        public override Type OutputPortType => typeof(AnswerChoiceConditionDialogueNodeView);
    }
}