using System.Collections.Generic;
using System.Linq;
using Module.InteractiveEditor.Configs;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class ImageListVisualController
    {
        private readonly VisualElement visualElement;
        private IReadOnlyCollection<ImageData> items;
        
        public ImageListVisualController(VisualElement visualElement)
        {
            this.visualElement = visualElement;
        }

        public void Init(IReadOnlyCollection<ImageData> items)
        {
            this.items = items;
            
            UpdateViewAsync();
        }

        private void UpdateViewAsync()
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

                var adrSprite = random.Image;

                if (adrSprite.RuntimeKeyIsValid)
                {
                    var guid = adrSprite.AssetGUID;
                    var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid));

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