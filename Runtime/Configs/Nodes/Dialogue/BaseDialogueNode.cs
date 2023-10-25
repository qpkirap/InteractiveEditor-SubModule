using UnityEngine;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Configs
{
    public class BaseDialogueNode : BaseNode
    {
        [SerializeField] private LocalizedString dialogue;
        
        public override NodeType NodeType => NodeType.Dialogue;
        protected override ExecuteResult ExecuteTask()
        {
            dialogue.GetLocalizedString();
            
            return ExecuteResult.SuccessState;
        }

        protected override ExecuteResult CancelTask()
        {
            return ExecuteResult.SuccessState;
        }
    }
}