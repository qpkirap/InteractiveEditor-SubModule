using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Module.InteractiveEditor.Configs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Module.InteractiveEditor.Runtime
{
    public class DefaultStoryTask : IStoryTask
    {
        private readonly PreloadManager preloadManager = new();
        private StoryObject storyObjectCache;
        private BaseNode currentNodeCache;
        
        private readonly Dictionary<Type, INodeExecute> executes = new();
        
        public StoryObject StoryObject => storyObjectCache;
        
        public async UniTask Init(StoryObject storyObject)
        {
            if (currentNodeCache != null)
            {
                await UniTask.WaitUntil(() => currentNodeCache == default 
                                              || currentNodeCache.ExecuteResult != ExecuteResult.RunningState);
            }
            
            if (storyObjectCache != null ) Object.DestroyImmediate(storyObjectCache);
            
            storyObjectCache = storyObject.Clone();
            
            InitExecutors(storyObjectCache);
            
            currentNodeCache = GetStartNode();
            
            preloadManager.PrepareNode(currentNodeCache);
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
            currentNodeCache = ExecuteNode(currentNodeCache);
            
            Debug.Log($"Execute Node {currentNodeCache?.Id}");
            
            preloadManager.PrepareNode(currentNodeCache);
        }

        public BaseNode ExecuteNode(BaseNode node)
        {
            if (node == null) return null;

            var calcNode = node;

            var executor = executes[calcNode.GetExecutorType()];
            
            if (executor == null) return null;

            switch (executor.Execute(calcNode))
            {
                case ExecuteResult.RunningState:
                {
                    return calcNode;
                }
                case ExecuteResult.SuccessState:
                {
                    var next = executor.GetNext(calcNode);
                    
                    executor.ResetExecutor(calcNode);
                    
                    return next;
                }
                case ExecuteResult.NoneState:
                {
                    return calcNode;
                }
            }

            return default;
        }

        public BaseNode GetStartNode()
        {
            if (StoryObject == null || StoryObject.Nodes == null) return null;

            if (string.IsNullOrEmpty(StoryObject.IdStartNode))
            {
                Debug.LogError($"Start node id is empty");
                
                var item = StoryObject.Nodes
                    .FirstOrDefault(x=> x != null 
                                        && x.ExecuteResult != ExecuteResult.SuccessState
                                        && x.ChildrenNodes.Count > 0);

                return item;
            }
            
            var startNode = StoryObject.Nodes
                .FirstOrDefault(x=> x != null
                                    && x.Id.Equals(StoryObject.IdStartNode));
            
            return startNode;
        }

        public void Dispose()
        {
            if (StoryObject != null)
            {
                Object.DestroyImmediate(StoryObject, true);
            }
        }
    }
}