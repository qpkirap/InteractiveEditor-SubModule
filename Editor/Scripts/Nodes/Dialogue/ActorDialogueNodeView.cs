using Module.InteractiveEditor.Configs;

namespace Module.InteractiveEditor.Editor
{
    [NodeView("Dialogue/ActorDialogue",typeof(ActorDialogueNode))]
    public class ActorDialogueNodeView : BaseDialogueNodeView
    {
        public ActorDialogueNodeView(BaseNode node) : base(node)
        {
        }
    }
}