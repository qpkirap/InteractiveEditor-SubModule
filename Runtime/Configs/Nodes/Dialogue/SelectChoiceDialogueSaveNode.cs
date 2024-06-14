using System;
using Module.InteractiveEditor.Runtime;
using Module.InteractiveEditor.Saves;

namespace Module.InteractiveEditor.Configs
{
    public class SelectChoiceDialogueSaveNode : SelectChoiceDialogueNode<DialogueSelectChoiceSaveExecutor>
    {
        [NonSerialized] private Type typeCache;
        
        public override Type GetSaveItemType()
        {
            if (typeCache == null)
            {
                typeCache = typeof(DialogueSelectChoiceSave);
            }
            
            return typeCache;
        }
    }
}