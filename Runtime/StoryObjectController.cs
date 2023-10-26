using System;
using System.Linq;
using Module.InteractiveEditor.Configs;
using UnityEngine;

namespace Module.InteractiveEditor.Runtime
{
    public class StoryObjectController : MonoBehaviour
    {
        [SerializeField] private StoryObject storyObject;
        
        public StoryObject StoryObject => storyObject;

        public void Init()
        {
            storyObject = storyObject.Clone();
        }
        
        private BaseNode ExecuteNode(BaseNode currentNode)
        {
            if (currentNode == null) return null;

            switch (currentNode.Execute())
            {
                case ExecuteResult.NoneState:
                    break;
                case ExecuteResult.RunningState:
                    break;
                case ExecuteResult.SuccessState:
                {
                    if(currentNode.ChildrenNodes is { Count: > 0 })
                    {
                        foreach (var childrenNode in currentNode.ChildrenNodes)
                        {
                            ExecuteNode(childrenNode);
                        }
                    }
                    
                    break;
                }
                case ExecuteResult.SkipState:
                    break;
                case ExecuteResult.ResetState:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return currentNode;
        }
    }
}