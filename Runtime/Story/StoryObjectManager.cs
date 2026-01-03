using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.DI;
using UniRx;
using VContainer;

namespace Module.InteractiveEditor.Runtime
{
    public class StoryObjectManager : IAsyncManagerInitializable
    {
        [Inject] private readonly StoryConfigs storyConfigs;
        [Inject] private readonly IObjectResolver resolver;
        
        private readonly ReactiveProperty<StoryObject> currentStoryObject = new();

        private readonly Subject onReload = new();
        
        public IReactiveProperty<StoryObject> CurrentStoryObject => currentStoryObject;
        public IObservable OnReload => onReload;

        public UniTask Init()
        {
            UpdateCurrentSelectedStoryObject();
            
            return UniTask.CompletedTask;
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
            
            var task = new DefaultStoryTask();
            resolver.Inject(task);
            return task;
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