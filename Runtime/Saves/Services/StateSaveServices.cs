using System.Collections.Generic;

namespace Module.InteractiveEditor.Saves
{
    public class StateSaveServices
    {
        private readonly SaveProvider saveProvider;
        private readonly Dictionary<string, ISavable> saves = new();
        
        public StateSaveServices(SaveProvider saveProvider)
        {
            this.saveProvider = saveProvider;
        }
        
        public void Init(ISavable[] saveItems)
        {
            if (saveItems == null) return;
            
            foreach (var saveItem in saveItems)
            {
                Add(saveItem);
            }
        }
        
        public void Add(ISavable saveItem)
        {
            if (!saves.TryAdd(saveItem.SaveKey, saveItem)) return;
            
            saveProvider.Add(saveItem);
        }
    }
}