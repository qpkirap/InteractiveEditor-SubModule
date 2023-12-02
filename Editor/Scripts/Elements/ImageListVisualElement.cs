using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class ImageListVisualElement
    {
        private readonly VisualElement visualElement;
        private readonly IEnumerable<AssetReference> items;
        
        public ImageListVisualElement(VisualElement visualElement, IEnumerable<AssetReference> items)
        {
            this.visualElement = visualElement;
            this.items = items;

            Init();
        }

        public void Init()
        {
            UpdateViewAsync();
        }

        private async UniTask UpdateViewAsync()
        {
            if (visualElement == null) return;

            if (items == null || items.All(x => x == null))
            {
                visualElement.style.backgroundImage = null;
                visualElement.style.height = 0;
            }
            else
            {
                var array = items.Where(x => x != null).ToArray();
                
                var random = array[Random.Range(0, array.Length)];
            
                var adrSprite = new AddressableSprite(random);
            
                var sprite = await adrSprite.LoadAsync();
            
                visualElement.style.backgroundImage = sprite.texture;
                visualElement.style.height = 100;
            }
        }
    }
}