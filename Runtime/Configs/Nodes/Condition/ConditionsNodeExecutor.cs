using System.Linq;
using Module.InteractiveEditor.Configs;

namespace Module.InteractiveEditor.Runtime
{
    public class ConditionsNodeExecutor : INodeExecute<ConditionsNode>
    {
        public BaseNode GetNext(ConditionsNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || !baseNode.ChildrenNodes.Any()) return null;
            
            return baseNode.ChildrenNodes[UnityEngine.Random.Range(0, baseNode.ChildrenNodes.Count)];
        }

        public ExecuteResult Execute(ConditionsNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public ExecuteResult Cancel(ConditionsNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(ConditionsNode baseNode)
        {
        }
    }
}