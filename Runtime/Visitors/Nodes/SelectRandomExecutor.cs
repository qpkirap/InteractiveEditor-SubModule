using System.Linq;
using Module.InteractiveEditor.Configs;

namespace Module.InteractiveEditor.Runtime
{
    public class SelectRandomExecutor : INodeExecute<SelectRandomBaseNode>
    {
        public BaseNode GetNext(SelectRandomBaseNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || !baseNode.ChildrenNodes.Any()) return null;
            
            return baseNode.ChildrenNodes[UnityEngine.Random.Range(0, baseNode.ChildrenNodes.Count)];
        }

        public ExecuteResult Execute(SelectRandomBaseNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public ExecuteResult Cancel(SelectRandomBaseNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(SelectRandomBaseNode baseNode)
        {
        }
    }
}