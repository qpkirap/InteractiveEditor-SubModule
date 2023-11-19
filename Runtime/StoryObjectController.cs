using System.Collections.Generic;
using System.Linq;
using Module.InteractiveEditor.Configs;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Module.InteractiveEditor.Runtime
{
    public class StoryObjectController : MonoBehaviour, IBaseController
    {
        private readonly LinkedList<BaseNode> history = new();
        private StoryObject storyObjectCache;
        private BaseNode currentNodeCache;
        
        public StoryObject StoryObject => storyObjectCache;
        
        public void Init()
        {
        }
        
        public void Init(StoryObject storyObject)
        {
            if (storyObjectCache != null) OnDestroy();
            
            storyObjectCache = storyObject.Clone();
            
            currentNodeCache ??= GetStartNode();
        }

        public void Disable()
        {
        }

        public BaseNode Tick()
        {
            currentNodeCache = ExecuteNode(currentNodeCache);

            return currentNodeCache;
        }
        
        private BaseNode ExecuteNode(BaseNode currentNode)
        {
            if (currentNode == null) return null;

            var calcNode = currentNode;
            
            if (calcNode != null) Debug.Log($"Execute: {calcNode.Id}");

            switch (currentNode.Execute())
            {
                case ExecuteResult.NoneState:
                    break;
                case ExecuteResult.RunningState:
                {
                    break;
                }
                case ExecuteResult.SuccessState:
                {
                    if (currentNode.ChildrenNodes is { Count: > 0 })
                    {
                        var filter = currentNode.ChildrenNodes
                            .Where(x => x.ExecuteResult == ExecuteResult.NoneState)
                            .ToArray();
                        
                        if (filter.Length > 0)
                        {
                            calcNode = filter[Random.Range(0, filter.Count())];
                        }
                        else
                        {
                            filter = currentNode.ChildrenNodes
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
                    currentNode.Cancel();

                    calcNode = currentNode;

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

        private BaseNode GetStartNode()
        {
            if (StoryObject == null || StoryObject.Nodes == null) return null;
            
            var item = StoryObject.Nodes.FirstOrDefault(x=> x != null && x.ExecuteResult != ExecuteResult.SuccessState);

            return item;
        }

        private void OnDestroy()
        {
            if (storyObjectCache != null) Object.DestroyImmediate(storyObjectCache);
        }
    }
}