using System;
using System.Collections.Generic;
using Module.InteractiveEditor.Configs;

namespace Module.InteractiveEditor.Saves
{
    public class NodeSaveServices : INodeSaveServices
    {
        private readonly SaveProvider saveProvider;

        private readonly Dictionary<string, SaveNodeItem> saveNodes = new();

        public NodeSaveServices(SaveProvider saveProvider)
        {
            this.saveProvider = saveProvider;
        }
        
        public SaveNodeItem GetSaveItem(string id)
        {
            return saveNodes.TryGetValue(id, out var saveItem) ? saveItem : default;
        }
        
        public void Init(IEnumerable<StoryObject> storyObject)
        {
            if (storyObject == null) return;

            foreach (var story in storyObject)
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
        }

        public void Add(ISavable saveItem)
        {
            if (saveItem is not SaveNodeItem) return;
            
            saveProvider.Add(saveItem);
        }
    }
}