using Module.InteractiveEditor.Configs;
using UnityEngine;
using UnityEngine.UI;

namespace Module.InteractiveEditor.Runtime
{
    public class StoryImageController : MonoBehaviour
    {
        [SerializeField] private Image image;
        
        private BaseNode currentNodeCache;

        public void ExecuteNode(BaseNode currentNode)
        {
            if (currentNode == null 
                || currentNodeCache != null && currentNodeCache.Id .Equals(currentNode.Id)) return;

            var random = currentNode.GetRandomItem;
            
            if (random == null) return;
        }
    }
}