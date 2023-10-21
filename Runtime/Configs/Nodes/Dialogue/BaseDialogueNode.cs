namespace Module.InteractiveEditor.Configs
{
    public class BaseDialogueNode : BaseNode
    {
        public override NodeType NodeType => NodeType.Dialogue;
        protected override ExecuteResult ExecuteTask()
        {
            return ExecuteResult.SuccessState;
        }

        protected override ExecuteResult CancelTask()
        {
            return ExecuteResult.SuccessState;
        }
    }
}