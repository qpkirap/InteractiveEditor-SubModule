using System;
using Module.InteractiveEditor.Saves;

namespace Module.InteractiveEditor.Configs
{
    public class AnswerChoiceConditionsSaveDialogueNode : AnswerChoiceConditionsDialogueNode<AnswerChoiceSaveDialogueNodeExecutor>
    {
        public override Type GetSaveItemType()
        {
            return typeof(AnswerChoiceSave);
        }
    }
}