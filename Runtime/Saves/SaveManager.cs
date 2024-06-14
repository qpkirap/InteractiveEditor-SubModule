using System.Collections.Generic;
using DepedencyInjection;
using Module.InteractiveEditor.Configs;

namespace Module.InteractiveEditor.Saves
{
    public class SaveManager
    {
        private readonly NodeSaveServices nodeSaveServices = new();
        
        public INodeSaveServices NodeSaveServices => nodeSaveServices;
        
        public SaveManager()
        {
            DI.Add(this);
        }
        
        public void Init(IEnumerable<StoryObject> storyObject)
        {
            nodeSaveServices.Init(storyObject);
            
            Load();
        }

        public void Add(ISavable saveItem)
        {
            nodeSaveServices.Add(saveItem);
        }

        public void Load()
        {
            nodeSaveServices.Load();
        }
        
        public void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                nodeSaveServices.Save();
            }
        }

        public void OnApplicationQuit()
        {
            nodeSaveServices.Save();
        }
    }
}