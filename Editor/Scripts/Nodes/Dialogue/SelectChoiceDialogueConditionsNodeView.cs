using System;
using Module.InteractiveEditor.Configs;
using UnityEditor.Experimental.GraphView;

namespace Module.InteractiveEditor.Editor
{
    [NodeView("Dialogue/Choices/Conditions/SelectChoiceDialogue",typeof(SelectChoiceDialogueConditionsNode))]
    public class SelectChoiceDialogueConditionsNodeView : SelectChoiceDialogueNodeView
    {
        public SelectChoiceDialogueConditionsNodeView(BaseNode node) : base(node)
        {
        }
        
        public override Port.Capacity InputPortCapacity => Port.Capacity.Multi;
        public override Port.Capacity OutputPortCapacity => Port.Capacity.Multi;

        public override Type OutputPortType => typeof(AnswerChoiceConditionsDialogueNodeView);
        
        public override string GetClassTag => "checkConditions";
    }
}