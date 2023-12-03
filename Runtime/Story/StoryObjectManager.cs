using System;
using DepedencyInjection;
using Module.InteractiveEditor.Configs;

namespace Module.InteractiveEditor.Runtime
{
    public class StoryObjectManager
    {
        private readonly IStoryTask defaultTask = new DefaultStoryTask();

        public StoryObjectManager()
        {
            DI.Add(this);
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
        void Init(StoryObject storyObject);
        void Execute();
    }
}