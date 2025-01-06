using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DepedencyInjection;
using Module.InteractiveEditor.Configs;
using Provider.Runtime;
using UniRx;

namespace Module.InteractiveEditor.Runtime
{
    public class StoryObjectManager
    {
        private readonly LazyInject<IConfigsProvider> configProvider = new();
        
        private readonly StoryConfigs storyConfigs;
        private readonly ReactiveProperty<StoryObject> currentStoryObject = new();

        private readonly Subject onReload = new();
        
        public IReactiveProperty<StoryObject> CurrentStoryObject => currentStoryObject;
        public IObservable OnReload => onReload;

        public StoryObjectManager()
        {
            DI.Add(this);

            configProvider.Value.GetConfig<StoryConfigs>();
            
            storyConfigs = configProvider.Value.GetConfig<StoryConfigs>();
            
            Init();
        }

        private void Init()
        {
            UpdateCurrentSelectedStoryObject();
        }
        
        private void UpdateCurrentSelectedStoryObject()
        {
            currentStoryObject.Value = storyConfigs.StoryObjects?.FirstOrDefault(x => x.StoryObject != null)?.StoryObject;
        }
        
        public void SetCurrentSelectedStoryObject(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            
            var storyObject = storyConfigs.StoryObjects.FirstOrDefault(x => x.StoryObject.Id.Equals(id));
            
            currentStoryObject.Value = storyObject?.StoryObject;
        }
        
        public IStoryTask GetTask(StoryObject obj)
        {
            if (obj == null) return null;
            
            return new DefaultStoryTask();
        }

        public void Reload()
        {
            onReload.OnNext();
        }
    }

    public interface IStoryTask : IDisposable
    {
        StoryObject StoryObject { get; }
        UniTask UnloadResources();
        UniTask Init(StoryObject storyObject);
        void Execute();
    }
}