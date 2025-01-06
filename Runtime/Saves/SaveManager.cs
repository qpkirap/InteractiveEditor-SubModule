using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DepedencyInjection;
using Module.InteractiveEditor.Configs;

namespace Module.InteractiveEditor.Saves
{
    public class SaveManager
    {
        private readonly SaveProvider saveProvider;
        private readonly NodeSaveServices nodeSaveServices;
        private readonly StateSaveServices stateSaveServices;
        
        public INodeSaveServices NodeSaveServices => nodeSaveServices;
        
        public SaveManager()
        {
            saveProvider = SaveProviderFactory.Create();
            nodeSaveServices = new NodeSaveServices(saveProvider);
            stateSaveServices = new StateSaveServices(saveProvider);
            
            DI.Add(this);
        }
        
        public async UniTask InitAsync(IEnumerable<StoryObject> storyObject, params ISavable[] saveItems)
        {
            nodeSaveServices.Init(storyObject);
            stateSaveServices.Init(saveItems);
            
           await ForceLoadAsync();
        }

        public void AddSaveNode(ISavable saveItem)
        {
            nodeSaveServices.Add(saveItem);
        }

        public async UniTask ForceLoadAsync()
        {
            await saveProvider.LoadAsync();
        }

        public void ForceSave()
        {
            saveProvider.Save();
        }

        public void Reset()
        {
            saveProvider.Reset();
        }
        
        public void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                saveProvider.Save();
            }
        }

        public void OnApplicationQuit()
        {
            saveProvider.Save();
        }
    }
}