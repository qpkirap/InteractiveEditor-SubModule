using System;
using Module.InteractiveEditor.Saves;

namespace Module.InteractiveEditor.Configs
{
    public class AnswerChoiceSaveDialogueNode : AnswerChoiceDialogueNode<AnswerChoiceSaveDialogueNodeExecutor>
    {
        public override Type GetSaveItemType()
        {
            return typeof(AnswerChoiceSave);
        }
    }
}