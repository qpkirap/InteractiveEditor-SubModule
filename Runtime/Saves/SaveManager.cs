using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Module.InteractiveEditor.DI;
using Module.InteractiveEditor.Runtime;
using VContainer;

namespace Module.InteractiveEditor.Saves
{
    /// <summary>
    /// Управляет сохранением и загрузкой данных.
    /// Реализует IAsyncManagerInitializable для автоматической инициализации.
    /// </summary>
    public class SaveManager : IAsyncManagerInitializable
    {
        [Inject] private readonly StoryConfigs storyConfigs;
        [Inject] private readonly IEnumerable<ISavable> savables;
        
        private SaveProvider saveProvider;
        private NodeSaveServices nodeSaveServices;
        private StateSaveServices stateSaveServices;
        
        public INodeSaveServices NodeSaveServices => nodeSaveServices;
        
        public async UniTask Init()
        {
            saveProvider = SaveProviderFactory.Create();
            nodeSaveServices = new NodeSaveServices(saveProvider);
            stateSaveServices = new StateSaveServices(saveProvider);
            var storyObjects = storyConfigs.StoryObjects
                .Where(x => x.StoryObject != null)
                .Select(x => x.StoryObject);
            
            nodeSaveServices.Init(storyObjects);
            stateSaveServices.Init(savables);
            
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