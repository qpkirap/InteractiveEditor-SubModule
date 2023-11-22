using System.Collections.Generic;
using System.Linq;
using Module.InteractiveEditor.Configs;
using UnityEngine;

namespace Module.InteractiveEditor.Runtime
{
    public class DefaultStoryTask : IStoryTask
    {
        private readonly LinkedList<BaseNode> history = new();
        private StoryObject storyObjectCache;
        private BaseNode currentNodeCache;
        
        public StoryObject StoryObject => storyObjectCache;
        
        public void Init(StoryObject storyObject)
        {
            history.Clear();
            
            if (storyObjectCache != null) Object.DestroyImmediate(storyObjectCache);
            
            storyObjectCache = storyObject.Clone();
            
            currentNodeCache ??= GetStartNode();
        }

        public BaseNode ExecuteNode(BaseNode node)
        {
            if (node == null) return null;

            var calcNode = node;
            
            if (calcNode != null) Debug.Log($"Execute: {calcNode.Id}");

            switch (node.Execute())
            {
                case ExecuteResult.NoneState:
                    break;
                case ExecuteResult.RunningState:
                {
                    break;
                }
                case ExecuteResult.SuccessState:
                {
                    if (node.ChildrenNodes is { Count: > 0 })
                    {
                        var filter = node.ChildrenNodes
                            .Where(x => x.ExecuteResult == ExecuteResult.NoneState)
                            .ToArray();
                        
                        if (filter.Length > 0)
                        {
                            calcNode = filter[Random.Range(0, filter.Count())];
                        }
                        else
                        {
                            filter = node.ChildrenNodes
                                .Where(x => x.ExecuteResult != ExecuteResult.SuccessState)
                                .ToArray();
                            
                            if (filter.Length > 0) calcNode = filter[Random.Range(0, filter.Count())];
                        }
                    }
                    
                    break;
                }
                case ExecuteResult.BackState:
                {
                    if (history.Count > 0)
                    {
                        calcNode = history.Last.Value;

                        calcNode.Cancel();
                        
                        history.RemoveLast();
                    }
                    
                    break;
                } 
                case ExecuteResult.ResetState:
                {
                    node.Cancel();

                    calcNode = node;

                    break;
                }
                default:
                    break;
            }

            if (calcNode != null && history.Count > 0 && !calcNode.Id.Equals(history.Last.Value.Id)
                || 
                calcNode != null && history.Count == 0)
            {
                history.AddLast(calcNode);
            }
            
            return calcNode != null ? calcNode.ExecuteResult != ExecuteResult.SuccessState ? calcNode : null : null;
        
        }

        public BaseNode GetStartNode()
        {
            if (StoryObject == null || StoryObject.Nodes == null) return null;
            
            var item = StoryObject.Nodes.FirstOrDefault(x=> x != null && x.ExecuteResult != ExecuteResult.SuccessState);

            return item;
        }

        public void Dispose()
        {
            if (StoryObject != null)
            {
                Object.DestroyImmediate(StoryObject);
            }
        }
    }
}