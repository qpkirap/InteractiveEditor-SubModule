using Module.InteractiveEditor.Configs;
using UnityEngine;

namespace Module.InteractiveEditor.Runtime
{
    public class StoryObjectController : MonoBehaviour, IBaseController, IStoryTask
    {
        private IStoryTask task;
        private BaseNode currentNodeCache;
        
        public StoryObject StoryObject => task.StoryObject;
        
        public void Init()
        {
        }

        public void Disable()
        {
        }

        public void Init(IStoryTask task)
        {
            this.task = task;

            currentNodeCache = GetStartNode();
        }
        
        public void Init(StoryObject storyObject)
        {
            task?.Init(storyObject);
            
            currentNodeCache = GetStartNode();
        }

        public void Tick()
        {
            currentNodeCache = ExecuteNode(currentNodeCache);
        }

        public BaseNode ExecuteNode(BaseNode node)
        {
            var currentNode = task?.ExecuteNode(node);

            return currentNode;
        }

        public BaseNode GetStartNode()
        {
            return task?.GetStartNode();
        }
        
        private void OnDestroy()
        {
            task?.Dispose();
        }

        public void Dispose()
        {
            task?.Dispose();
        }
    }
}