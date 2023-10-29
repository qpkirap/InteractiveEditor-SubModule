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

        public BaseNode Traverse(BaseNode firstNode, Action<BaseNode> visitor)
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

            return null;
        }

        public StoryObject Clone()
        {
            var story = CreateInstance<StoryObject>();

            story.nodes ??= new();

            if (nodes != null)
            {
                var first = nodes.FirstOrDefault(x=> x.ChildrenNodes is { Count: > 0 });

                if (first != null)
                {
                    var clone = (BaseNode)first.Clone();

                    Traverse(clone, (cloneNode) =>
                    {
                        story.nodes.Add(cloneNode);
                    });
                }
            }

            return story;
        }
    }
}