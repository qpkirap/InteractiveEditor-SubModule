using System.Collections.Generic;
using System.Linq;
using Module.InteractiveEditor.Configs;
using UnityEngine;

namespace Module.InteractiveEditor.Runtime
{
    public class StoryObjectController : MonoBehaviour
    {
        [SerializeField] private StoryObject storyObject;
        
        private readonly LinkedList<BaseNode> history = new();
        private BaseNode currentNodeCache;
        
        public StoryObject StoryObject => storyObject;

        public void Init()
        {
            storyObject = storyObject.Clone();
        }

        private void Tick()
        {
            currentNodeCache ??= GetStartNode();

            currentNodeCache = ExecuteNode(currentNodeCache);
        }
        
        private BaseNode ExecuteNode(BaseNode currentNode)
        {
            if (currentNode == null) return null;

            var calcNode = currentNode;

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
                        calcNode = currentNode.ChildrenNodes.FirstOrDefault(x =>
                            x.ExecuteResult == ExecuteResult.NoneState);
                    }
                    
                    break;
                }
                case ExecuteResult.BackState:
                {
                    if (history.Count > 0)
                    {
                        calcNode = history.Last.Value;
                        
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

            if (calcNode != null && !calcNode.Id.Equals(history.Last.Value.Id))
            {
                history.AddAfter(history.Last, calcNode);
            }
            
            return calcNode;
        }

        private BaseNode GetStartNode()
        {
            if (StoryObject == null || StoryObject.Nodes == null) return null;
            
            var item = StoryObject.Nodes.FirstOrDefault(x=> x != null);

            return item;
        }
    }
}