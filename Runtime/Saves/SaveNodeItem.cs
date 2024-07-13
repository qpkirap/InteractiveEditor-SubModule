using System;
using Module.InteractiveEditor.Configs;
using Newtonsoft.Json;

namespace Module.InteractiveEditor.Saves
{
    [Serializable]
    public abstract class SaveNodeItem<TNode> : SaveNodeItem
        where TNode : BaseNode
    {
        [JsonIgnore] protected TNode baseNode;
        
        public override string SaveKey { get; }
        
        public SaveNodeItem(TNode baseNode)
        {
            this.baseNode = baseNode;
            
            SaveKey = baseNode.Id;
        }
        
        public abstract void UpdateData();
    }

    public abstract class SaveNodeItem : ISavable
    {
        public abstract string SaveKey { get; }
        
        public void PostLoad()
        {
        }

        public bool Equals(ISavable other)
        {
            if (other == null) return false;
            
            return SaveKey == other.SaveKey;
        }
    }
}