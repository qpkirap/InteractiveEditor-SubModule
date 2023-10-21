using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    [CreateAssetMenu]
    public class StoryObject : BaseConfig
    {
        [SerializeField] private List<BaseNode> nodes;

        public const string NodesKey = nameof(nodes);

        public IReadOnlyList<BaseNode> Nodes => nodes;

        public void Traverse(BaseNode firstNode, Action<BaseNode> visitor)
        {
            if (firstNode)
            {
                visitor.Invoke(firstNode);
                
                var children = firstNode.ChildrenNodes;

                foreach (var baseNode in children)
                {
                    Traverse(baseNode, visitor);
                }
            }
        }

        public StoryObject Clone()
        {
            var story = Instantiate(this);

            return story;
        }
    }
}