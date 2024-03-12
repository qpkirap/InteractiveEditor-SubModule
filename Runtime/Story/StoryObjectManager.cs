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
        private readonly IStoryTask defaultTask = new DefaultStoryTask();
        private readonly ReactiveProperty<StoryObject> currentStoryObject = new();
        
        public IReactiveProperty<StoryObject> CurrentStoryObject => currentStoryObject;

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
            
            return defaultTask;
        }
    }

    public interface IStoryTask : IDisposable
    {
        StoryObject StoryObject { get; }
        UniTask Init(StoryObject storyObject);
        void Execute();
    }
}