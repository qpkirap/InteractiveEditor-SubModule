using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DepedencyInjection;
using Managers.Router;
using Managers.Router.Config;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Saves;

namespace Module.InteractiveEditor.Runtime
{
#if UNITY_WEBGL
    
#endif
    public class DefaultStoryTask : IStoryTask
    {
        private readonly PreloadManager preloadManager = new();
        private readonly LazyInject<SaveManager> saveManager = new();
        private readonly LazyInject<IRouter> router = new();
        private readonly Dictionary<Type, INodeExecute> executes = new();
        
        private StoryObject storyObjectCache;
        private BaseNode currentNodeCache;
        
        private int currentDepth = 0;
        
        public StoryObject StoryObject => storyObjectCache;

        public async UniTask UnloadResources()
        {
            await preloadManager.UnloadAllAssets();
        }
        
        public async UniTask Init(StoryObject storyObject)
        {
            router.Value.ShowLoadingScreen(LoadingScreenKeys.start);
            
            if (currentNodeCache != null)
            {
                await UniTask.WaitUntil(() => currentNodeCache == default 
                                              || currentNodeCache.ExecuteResult != ExecuteResult.RunningState);
            }
            
            storyObjectCache = storyObject;
            
            InitExecutors(storyObjectCache);
            
            currentNodeCache = GetStartNode();
            
            await preloadManager.InitStory(storyObject, currentNodeCache);
            
            router.Value.HideLoadingScreen();
        }

        private void InitExecutors(StoryObject storyObject)
        {
            if (storyObject == null) return;
            
            foreach (var node in storyObject.Nodes)
            {
                if (node == null) continue;
                
                var executorType = node.GetExecutorType();
           
                if (executorType == null) continue;

                if (executes.TryGetValue(executorType, out var execute))
                {
                    execute.ResetExecutor(node);
                    continue;
                }
                
                var executor = (INodeExecute)Activator.CreateInstance(executorType);
            
                executes.Add(executorType, executor);
            }
        }
        
        public void Execute()
        {
            var (currentNode, result) = ExecuteNode(currentNodeCache);

            currentNodeCache = currentNode;

            if (result == ExecuteResult.SuccessState)
            {
                currentDepth++;
                
                preloadManager.PrepareAssets(currentDepth, storyObjectCache).Forget();
            }
        }

        public (BaseNode nextNode, ExecuteResult result) ExecuteNode(BaseNode node)
        {
            if (node == null) return default;

            var calcNode = node;

            var executor = executes[calcNode.GetExecutorType()];
            
            if (executor == null) return default;

            switch (executor.Execute(calcNode))
            {
                case ExecuteResult.RunningState:
                {
                    return (calcNode, ExecuteResult.RunningState);
                }
                case ExecuteResult.SuccessState:
                {
                    var next = executor.GetNext(calcNode);
                    
                    executor.ResetExecutor(calcNode);
                    
                    saveManager.Value.NodeSaveServices.SetLastIdNode(node.Id);
                    
                    saveManager.Value.ForceSave();
                    
                    return (next, ExecuteResult.SuccessState);
                }
                case ExecuteResult.NoneState:
                {
                    return (calcNode, ExecuteResult.NoneState);
                }
            }

            return default;
        }

        public BaseNode GetStartNode()
        {
            if (StoryObject == null || StoryObject.Nodes == null) return null;

            var lastId = saveManager.Value.NodeSaveServices.GetIdLastNode(StoryObject.Id);
            
            var startNode = StoryObject.Nodes
                .FirstOrDefault(x=> x != null
                                    && x.Id.Equals(lastId));
            
            return startNode;
        }

        public void Dispose()
        {
        }
    }
}