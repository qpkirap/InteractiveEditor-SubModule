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
            
            defaultTask.Init(obj);
            
            return defaultTask;
        }
    }

    public interface IStoryTask : IDisposable
    {
        StoryObject StoryObject { get; }
        void Init(StoryObject storyObject);
        
        BaseNode ExecuteNode(BaseNode node);
        BaseNode GetStartNode();
    }
}