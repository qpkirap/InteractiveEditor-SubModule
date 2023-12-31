using System;
using Cysharp.Threading.Tasks;
using Module.InteractiveEditor.Configs;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class ImageDataViewItem : Button
    {
        private VisualElement imageElement;
        
        public ImageData ImageData { get; private set; }
        
        public ImageDataViewItem()
        {
            AddToClassList("image-data-item");
        }

        public void InjectData(ImageData imageData)
        {
            this.ImageData = imageData;
            
            text = imageData.Title;

            CreateImageElementAsync(imageData);
        }
        
        private async UniTask CreateImageElementAsync(ImageData imageData)
        {
            if (imageData == null) return;
            
            imageElement = new VisualElement();
            
            imageElement.AddToClassList("episode-image");
            
            Add(imageElement);
            
            if (imageData.Image is not { RuntimeKeyIsValid: true }) return;

            try
            {
                var sprite = await imageData.Image.LoadAsync();
                
                if (sprite == null) return;
            
                imageElement.style.backgroundImage = new StyleBackground(sprite);
            }
            catch (Exception e)
            {
                // TODO: Log
            }
        }
    }
}