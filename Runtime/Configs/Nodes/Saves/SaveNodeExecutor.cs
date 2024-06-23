using System.Linq;
using DepedencyInjection;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Saves;

namespace Module.InteractiveEditor.Runtime
{
    public class SaveNodeExecutor  : INodeExecute<SaveNode>
    {
        private readonly LazyInject<SaveManager> saveManager = new();
        
        public BaseNode GetNext(SaveNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || !baseNode.ChildrenNodes.Any()) return null;
            
            return baseNode.ChildrenNodes[UnityEngine.Random.Range(0, baseNode.ChildrenNodes.Count)];
        }

        public ExecuteResult Execute(SaveNode baseNode)
        {
            saveManager.Value.NodeSaveServices.SetLastIdNode(baseNode.Id);
            saveManager.Value.ForceSave();
            
            return ExecuteResult.SuccessState;
        }

        public ExecuteResult Cancel(SaveNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(SaveNode baseNode)
        {
        }
    }
}