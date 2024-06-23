using System;
using System.Collections.Generic;
using System.Linq;
using Module.InteractiveEditor.Configs;
using Newtonsoft.Json;
using UnityEngine;

namespace Module.InteractiveEditor.Saves
{
    public class NodeSaveServices : INodeSaveServices
    {
        private readonly SaveProvider saveProvider;
        private readonly SaveLastNodeState saveLastNodeState = new();

        private readonly Dictionary<string, SaveNodeItem> saveNodes = new();

        public NodeSaveServices(SaveProvider saveProvider)
        {
            this.saveProvider = saveProvider;
        }
        
        public void Init(IEnumerable<StoryObject> storyObjects)
        {
            if (storyObjects == null) return;

            foreach (var story in storyObjects)
            {
                if (story == null || story.Nodes == null) continue;

                foreach (var storyNode in story.Nodes)
                {
                    if (storyNode == null || saveNodes.ContainsKey(storyNode.Id)) continue;

                    var saveItemType = storyNode.GetSaveItemType();
                    
                    if (saveItemType == default) continue;
                    
                    var saveItem = (SaveNodeItem)Activator.CreateInstance(saveItemType);
                    
                    saveNodes.Add(storyNode.Id, saveItem);
                    
                    Add(saveItem);
                }
            }
            
            saveLastNodeState.Init(storyObjects);
            saveProvider.Add(saveLastNodeState);
        }

        public void Add(ISavable saveItem)
        {
            if (saveItem is not SaveNodeItem) return;
            
            saveProvider.Add(saveItem);
        }
        
        public SaveNodeItem GetSaveItem(string id)
        {
            return saveNodes.TryGetValue(id, out var saveItem) ? saveItem : default;
        }
        
        public string GetIdLastNode(string idStory)
        {
            return saveLastNodeState.GetLastNode(idStory);
        }

        public void SetLastIdNode(string idStory, string idNode)
        {
            saveLastNodeState.SetLast(idStory, idNode);
        }

        public void SetLastIdNode(string idNode)
        {
            saveLastNodeState.SetLast(idNode);
        }
    }
    
    [Serializable]
    public class SaveLastNodeState : ISavable
    {
        [JsonProperty("StoryLastNodes")] private Dictionary<string, string> StoryLastNodes = new();

        [JsonIgnore] public string SaveKey => nameof(SaveLastNodeState);

        [JsonIgnore] private List<StoryObject> storyObjectsCache;

        public void Init(IEnumerable<StoryObject> storyObjects)
        {
            if (storyObjects == null) return;
            
            storyObjectsCache = storyObjects.ToList();
            
            UpdateIds();
        }
        
        public void SetLast(string idStory, string idNode)
        {
            if (string.IsNullOrEmpty(idNode) || string.IsNullOrEmpty(idStory)) return;
            
            StoryLastNodes[idStory] = idNode;
        }

        public void SetLast(string idNode)
        {
            if (string.IsNullOrEmpty(idNode)) return;
            
            if (storyObjectsCache == null) return;
            
            foreach (var storyObject in storyObjectsCache)
            {
                if (storyObject == null || storyObject.Nodes == null) continue;
                
                var startNode = storyObject.Nodes
                    .FirstOrDefault(x=> x != null
                                        && x.Id.Equals(idNode));
                
                SetLast(storyObject.Id, startNode?.Id);
            }
        }
        
        public string GetLastNode(string idStory)
        {
            if (string.IsNullOrEmpty(idStory)) return string.Empty;
            
            return StoryLastNodes.TryGetValue(idStory, out var idNode) ? idNode : string.Empty;
        }

        public bool Equals(ISavable other)
        {
            if (other == null) return false;
            
            return SaveKey == other.SaveKey;
        }

        public void PostLoad()
        {
            UpdateIds();
        }

        private void UpdateIds()
        {
            if (storyObjectsCache == null) return;
            
            foreach (var storyObject in storyObjectsCache)
            {
                if (storyObject == null || storyObject.Nodes == null || StoryLastNodes.ContainsKey(storyObject.Id)) continue;
                
                if (string.IsNullOrEmpty(storyObject.IdStartNode))
                {
                    Debug.LogError($"Start node id is empty");
                
                    var item = storyObject.Nodes
                        .FirstOrDefault(x=> x != null 
                                            && x.ExecuteResult != ExecuteResult.SuccessState
                                            && x.ChildrenNodes.Count > 0);

                    SetLast(storyObject.Id, item.Id);
                }
                else
                {
                    var startNode = storyObject.Nodes
                        .FirstOrDefault(x=> x != null
                                            && x.Id.Equals(storyObject.IdStartNode));
                    
                    SetLast(storyObject.Id, startNode?.Id);
                }
            }
        }
    }
}