﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using Module.Utils;
using Module.Utils.Configs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Module.InteractiveEditor.Configs
{
    [CreateAssetMenu][Serializable]
    public class StoryObject : BaseConfig
    {
        [FoldoutGroup("Story Content")]
        [LabelText("Story Nodes")]
        [SerializeField, Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)] 
        private List<BaseNode> nodes;
        
        [FoldoutGroup("Story Content")]
        [LabelText("Actors")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "title")]
        [SerializeField] 
        private List<Actor> actors;
        
        [FoldoutGroup("Story Content")]
        [LabelText("Episodes")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "title")]
        [SerializeField] 
        private List<EpisodeData> episodeDatas;
        
        [FoldoutGroup("Story Settings")]
        [LabelText("Start Node ID")]
        [SerializeField] 
        private string idStartNode;

        #region Editor

        [HideInInspector][SerializeField] private Vector3 viewPositionEditor;
        [HideInInspector][SerializeField] private Vector3 viewScaleEditor;

        #endregion

        public const string NodesKey = nameof(nodes);
        public const string ActorsKey = nameof(actors);
        public const string IdStartNodeKey = nameof(idStartNode);
        public const string ViewPositionKey = nameof(viewPositionEditor);
        public const string ViewScaleKey = nameof(viewScaleEditor);
        public const string EpisodeDatasKey = nameof(episodeDatas);

        public IReadOnlyList<BaseNode> Nodes => nodes;
        public IReadOnlyList<Actor> Actors => actors;
        public IReadOnlyList<EpisodeData> Episodes => episodeDatas;
        public string IdStartNode => idStartNode;

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
            var storyClone = CreateInstance<StoryObject>();

            storyClone.nodes ??= new();

            if (this.nodes != null)
            {
                var first = nodes.FirstOrDefault(x=> x.Id.Equals(IdStartNode));

                if (first != null)
                {
                    var clone = (BaseNode)first.Clone();
                    
                    clone.RemoveCloneSuffix();
                    clone.SetId(first.Id);

                    Traverse(clone, (cloneNode) =>
                    {
                        storyClone.nodes.Add(cloneNode);
                    });
                }
            }
            
            if (string.IsNullOrEmpty(Id)) GenerateId();
            
            storyClone.SetId(Id);
            storyClone.SetFieldValue(StoryObject.IdStartNodeKey, IdStartNode);

            return storyClone;
        }
    }
}