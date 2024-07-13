using Module.InteractiveEditor.Configs;

namespace Module.InteractiveEditor.Editor
{
    [NodeView("Dialogue/Choices/SaveAnswerChoice", typeof(AnswerChoiceSaveDialogueNode))]
    public class AnswerChoiceSaveDialogueNodeView : AnswerChoiceDialogueNodeView
    {
        public override string GetClassTag => "save";
        
        public AnswerChoiceSaveDialogueNodeView(BaseNode node) : base(node)
        {
        }
    }
}