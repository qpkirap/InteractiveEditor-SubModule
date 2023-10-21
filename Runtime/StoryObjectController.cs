using Module.InteractiveEditor.Configs;
using UnityEngine;

namespace Module.InteractiveEditor.Runtime
{
    public class StoryObjectController : MonoBehaviour
    {
        [SerializeField] private StoryObject storyObject;
        
        public StoryObject StoryObject => storyObject;

        private void Start()
        {
            storyObject = storyObject.Clone();
        }
    }
}