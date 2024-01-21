using System;
using Module.InteractiveEditor.Runtime;

namespace Module.InteractiveEditor.Configs
{
    public class SelectChoiceDialogueNode : BaseDialogueNode<DialogueSelectChoiceExecutor>
    {
    }
    
    public class SelectChoiceDialogueNode<T> : SelectChoiceDialogueNode
        where T : INodeExecute
    {
        public override Type GetExecutorType()
        {
            return typeof(T);
        }
    }
}