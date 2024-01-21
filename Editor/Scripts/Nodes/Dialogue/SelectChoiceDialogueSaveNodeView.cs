using Module.InteractiveEditor.Configs;
using UnityEditor.Experimental.GraphView;

namespace Module.InteractiveEditor.Editor
{
    [NodeView("Dialogue/Choices/Save/SelectChoiceDialogue",typeof(SelectChoiceDialogueSaveNode))]
    public class SelectChoiceDialogueSaveNodeView : SelectChoiceDialogueNodeView
    {
        public SelectChoiceDialogueSaveNodeView(BaseNode node) : base(node)
        {
        }
        
        public override Port.Capacity InputPortCapacity => Port.Capacity.Multi;
        public override Port.Capacity OutputPortCapacity => Port.Capacity.Multi;
    }
}