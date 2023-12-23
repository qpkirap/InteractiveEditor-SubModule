using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Module.InteractiveEditor.Runtime;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class ImageListVisualController
    {
        private readonly VisualElement visualElement;
        private readonly CancellationTokenHandler tokenHandler = new();
        private IReadOnlyCollection<AssetReference> items;
        
        public ImageListVisualController(VisualElement visualElement)
        {
            this.visualElement = visualElement;
        }

        public void Init(IReadOnlyCollection<AssetReference> items)
        {
            this.items = items;
            
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

                if (adrSprite.RuntimeKeyIsValid)
                {
                    var sprite = await adrSprite.LoadAsync(token: tokenHandler.Token);
                    
                    if (tokenHandler.Token.IsCancellationRequested) return;

                    if (sprite == null)
                    {
                        visualElement.style.backgroundImage = null;
                        visualElement.style.height = 0;
                    }
            
                    visualElement.style.backgroundImage = sprite.texture;
                    visualElement.style.height = 100;
                    
                    visualElement.MarkDirtyRepaint();
                }
                else
                {
                    visualElement.style.backgroundImage = null;
                    visualElement.style.height = 0;
                }
            }
        }
    }
}