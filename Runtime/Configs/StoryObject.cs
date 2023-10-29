using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    [CreateAssetMenu]
    public class StoryObject : BaseConfig
    {
        [SerializeField] private List<BaseNode> nodes;

        public const string NodesKey = nameof(nodes);

        public IReadOnlyList<BaseNode> Nodes => nodes;

        public BaseNode Traverse(BaseNode firstNode, Func<BaseNode, BaseNode> visitor)
        {
            if (firstNode)
            {
                var test = visitor.Invoke(firstNode);
                
                var children = firstNode.ChildrenNodes;

                foreach (var baseNode in children)
                {
                    Traverse(baseNode, visitor);
                }

                return test;
            }

            return null;
        }

        public StoryObject Clone()
        {
            var story = CreateInstance<StoryObject>();

            story.nodes ??= new();

            if (nodes != null)
            {
                foreach (var baseNode in nodes)
                {
                    if (baseNode == null) continue;
                    
                    var first = Traverse(baseNode, (node) =>
                    {
                        var clone = (BaseNode)node.Clone();
                        
                        clone.AddToList(BaseNode.ChildNodeKey, clone);

                        return clone;
                    });
                    
                    story.AddToList(NodesKey, first);
                }
            }

            return story;
        }
    }
}