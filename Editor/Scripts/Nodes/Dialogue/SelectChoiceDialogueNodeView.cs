using System;
using Module.InteractiveEditor.Configs;
using UnityEditor.Experimental.GraphView;

namespace Module.InteractiveEditor.Editor
{
    [NodeView("Dialogue/Choices/SelectChoiceDialogue",typeof(SelectChoiceDialogueNode))]
    public class SelectChoiceDialogueNodeView : BaseDialogueNodeView
    {
        public SelectChoiceDialogueNodeView(BaseNode node) : base(node)
        {
        }
        
        public override Type InputPortType => typeof(bool);
        public override Type OutputPortType => typeof(AnswerChoiceDialogueNodeView);
        public override Port.Capacity InputPortCapacity => Port.Capacity.Single;
        public override Port.Capacity OutputPortCapacity => Port.Capacity.Multi;
    }
}