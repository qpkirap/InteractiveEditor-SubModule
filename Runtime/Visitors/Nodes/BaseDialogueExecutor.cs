using System.Linq;
using DepedencyInjection;
using Managers.Router;
using Managers.Router.Config;
using Module.InteractiveEditor.Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Module.InteractiveEditor.Runtime
{
    public class BaseDialogueExecutor : INodeExecute<BaseDialogueNode>
    {
        private static LazyInject<IRouter> router = new();

        private BaseDialogueNode node;
        private AddressableSprite background;
        
        private bool isOpenCanvas;
        private bool isNext;
        
        public ExecuteResult Execute(BaseDialogueNode baseNode)
        {
            node ??= baseNode;
            
            if (!isOpenCanvas)
            {
                router.Value.GoTo(RoutKeys.baseDialogue, routArgs: new (string, object)[]
                {
                    (INodeExecute.NodeExecutorKey, this)
                });
                
                isOpenCanvas = true;
            }
            
            return isNext ? ExecuteResult.SuccessState : ExecuteResult.RunningState;
        }

        public ExecuteResult Cancel(BaseDialogueNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(BaseDialogueNode baseNode)
        {
            node = null;
            isNext = false;
            isOpenCanvas = false;
            
            background?.Release();

            background = null;
        }
        
        public AddressableSprite GetBackground()
        {
            background ??= node.RandomImage;
            
            return background;
        }

        public void Complete()
        {
            isNext = true;
        }

        public BaseNode GetNext(BaseDialogueNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || !baseNode.ChildrenNodes.Any()) return null;
            
            return baseNode.ChildrenNodes[Random.Range(0, baseNode.ChildrenNodes.Count())];
        }
    }
}