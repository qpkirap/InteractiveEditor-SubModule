using System;
using Module.InteractiveEditor.Saves;

namespace Module.InteractiveEditor.Configs
{
    public class AnswerChoiceSaveDialogueNode : AnswerChoiceDialogueNode<AnswerChoiceSaveDialogueNodeExecutor<AnswerChoiceSaveDialogueNode>>
    {
        public override Type GetSaveItemType()
        {
            return typeof(AnswerChoiceSave);
        }
    }
}