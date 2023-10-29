using UnityEngine;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Configs
{
    public class BaseDialogueNode : BaseNode
    {
        [SerializeField] private LocalizedString dialogue;
        
        private const string DialogueKey = nameof(dialogue);
        
        public override NodeType NodeType => NodeType.Dialogue;
        
        protected override ExecuteResult ExecuteTask()
        {
            if (dialogue is { IsEmpty: false }) dialogue.GetLocalizedString();
            
            return ExecuteResult.SuccessState;
        }

        protected override ExecuteResult CancelTask()
        {
            return ExecuteResult.SuccessState;
        }

        public override object Clone()
        {
            var item =  base.Clone();
            
            item.SetFieldValue(DialogueKey, dialogue);

            return item;
        }
    }
}