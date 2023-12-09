using System.Linq;
using Module.InteractiveEditor.Configs;
using UnityEngine;

namespace Module.InteractiveEditor.Runtime
{
    public class BaseDialogueExecutor : INodeExecute<BaseDialogueNode>
    {
        public ExecuteResult Execute(BaseDialogueNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public ExecuteResult Cancel(BaseDialogueNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(BaseDialogueNode baseNode)
        {
        }

        public BaseNode GetNext(BaseDialogueNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || !baseNode.ChildrenNodes.Any()) return null;
            
            return baseNode.ChildrenNodes[Random.Range(0, baseNode.ChildrenNodes.Count())];
        }
    }
}