using Module.InteractiveEditor.Configs;

namespace Module.InteractiveEditor.Editor
{
    [NodeView("Dialogue/Choices/Conditions/SaveAnswerChoiceConditions", typeof(AnswerChoiceConditionsSaveDialogueNode))]
    public class AnswerChoiceConditionsSaveDialogueNodeView : AnswerChoiceConditionsDialogueNodeView
    {
        public AnswerChoiceConditionsSaveDialogueNodeView(BaseNode node) : base(node)
        {
        }

        public override string GetClassTag => "save";
    }
}