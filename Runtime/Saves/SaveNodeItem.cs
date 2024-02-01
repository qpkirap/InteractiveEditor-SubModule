using System;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Runtime;
using Newtonsoft.Json;

namespace Module.InteractiveEditor.Saves
{
    [Serializable]
    public abstract class SaveNodeItem<TNodeExecutor> : SaveNodeItem
        where TNodeExecutor : INodeExecute
    {
        [JsonIgnore] protected BaseNode baseNode;
        [JsonIgnore] protected TNodeExecutor executor;
        
        public override string SaveKey { get; }
        
        public SaveNodeItem(TNodeExecutor executor, BaseNode baseNode)
        {
            this.baseNode = baseNode;
            this.executor = executor;
            
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