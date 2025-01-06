using System.Linq;
using DepedencyInjection;
using Managers.Router;
using Managers.Router.Config;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Saves;
using UnityEngine;

namespace Module.InteractiveEditor.Runtime
{
    public class EndGameExecutor : INodeExecute<EndGameNode>
    {
        private static LazyInject<IRouter> router = new();
        private static LazyInject<StoryObjectManager> storyManager = new();
        private static LazyInject<SaveManager> saveManager = new();
        
        private EndGameNode node;
        
        private bool isOpenCanvas;
        private bool isNext;
        
        public BaseNode GetNext(EndGameNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || !baseNode.ChildrenNodes.Any()) return null;
            
            return baseNode.ChildrenNodes[Random.Range(0, baseNode.ChildrenNodes.Count())];
        }

        public ExecuteResult Execute(EndGameNode baseNode)
        {
            node ??= baseNode;
            
            if (!isOpenCanvas)
            {
                router.Value.GoTo(RoutKeys.endGame, routArgs: new (string, object)[]
                {
                    (INodeExecute.NodeExecutorKey, this)
                });
                
                isOpenCanvas = true;
            }
            
            return isNext ? ExecuteResult.SuccessState : ExecuteResult.RunningState;
        }

        public ExecuteResult Cancel(EndGameNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(EndGameNode baseNode)
        {
            node = null;
            isNext = false;
            isOpenCanvas = false;
        }
        
        public void Complete()
        {
            saveManager.Value.NodeSaveServices.SetLastIdNode(string.Empty);
            saveManager.Value.Reset();
            storyManager.Value.Reload();
            
            isNext = true;
        }
    }
}