using Module.InteractiveEditor.Configs;

namespace Module.InteractiveEditor.Editor
{
    [NodeView(typeof(ActorDialogueNode))]
    public class ActorDialogueNodeView : BaseDialogueNodeView
    {
        public ActorDialogueNodeView(BaseNode node) : base(node)
        {
        }
    }
}