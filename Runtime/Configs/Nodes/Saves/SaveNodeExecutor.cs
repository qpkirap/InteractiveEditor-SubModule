using System.Linq;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Saves;
using VContainer;

namespace Module.InteractiveEditor.Runtime
{
    public class SaveNodeExecutor : INodeExecute<SaveNode>
    {
        [Inject] private readonly SaveManager saveManager;
        
        public BaseNode GetNext(SaveNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || !baseNode.ChildrenNodes.Any()) return null;
            
            return baseNode.ChildrenNodes[UnityEngine.Random.Range(0, baseNode.ChildrenNodes.Count)];
        }

        public ExecuteResult Execute(SaveNode baseNode)
        {
            saveManager.NodeSaveServices.SetLastIdNode(baseNode.Id);
            saveManager.ForceSave();
            
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