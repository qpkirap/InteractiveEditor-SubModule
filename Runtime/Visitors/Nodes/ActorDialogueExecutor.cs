using System.Collections.Generic;
using System.Linq;
using DepedencyInjection;
using Managers.Router;
using Managers.Router.Config;
using Module.InteractiveEditor.Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Runtime
{
    public class ActorDialogueExecutor : INodeExecute<ActorDialogueNode>
    {
        private static LazyInject<IRouter> router = new();

        private ImageData imageDataCache;
        private ActorDialogueNode node;
        private AddressableSprite background;
        
        private bool isOpenCanvas;
        private bool isNext;
        
        public AddressableSprite GetBackground()
        {
            var data = GetImageData();
            
            if (data == null) return null;
            
            background ??= data.Image;
            
            return background;
        }

        public IReadOnlyList<CensureData> GetCensure()
        {
            return GetImageData()?.Censures;
        }

        private ImageData GetImageData()
        {
            imageDataCache ??= node.RandomImage;

            return imageDataCache;
        }
        
        public LocalizedString GetText()
        {
            return node.Dialogue;
        }

        public Actor GetActor()
        {
            return node.Actor;
        }
        
        public void Complete()
        {
            isNext = true;
        }
        
        public BaseNode GetNext(ActorDialogueNode baseNode)
        {
            if (baseNode.ChildrenNodes == null || !baseNode.ChildrenNodes.Any()) return null;
            
            return baseNode.ChildrenNodes[Random.Range(0, baseNode.ChildrenNodes.Count())];
        }

        public ExecuteResult Execute(ActorDialogueNode baseNode)
        {
            node ??= baseNode;
            
            if (!isOpenCanvas)
            {
                router.Value.GoTo(RoutKeys.actorDialogueCanvas, routArgs: new (string, object)[]
                {
                    (INodeExecute.NodeExecutorKey, this)
                });
                
                isOpenCanvas = true;
            }
            
            return isNext ? ExecuteResult.SuccessState : ExecuteResult.RunningState;
        }

        public ExecuteResult Cancel(ActorDialogueNode baseNode)
        {
            return ExecuteResult.SuccessState;
        }

        public void ResetExecutor(ActorDialogueNode baseNode)
        {
            node = null;
            isNext = false;
            isOpenCanvas = false;
            
            background?.Release();

            background = null;
            imageDataCache = null;
        }
    }
}