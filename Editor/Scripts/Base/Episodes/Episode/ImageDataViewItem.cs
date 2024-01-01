using System;
using Cysharp.Threading.Tasks;
using Module.InteractiveEditor.Configs;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class ImageDataViewItem : Button
    {
        private VisualElement image;
        
        public ImageData ImageData { get; private set; }
        
        public ImageDataViewItem()
        {
            AddToClassList("image-data-item");
            
            image = new VisualElement();
            
            image.AddToClassList("episode-image");
            image.style.width = 0;
            image.style.height = 0;
            
            Add(image);
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
            
            if (imageData.Image is not { RuntimeKeyIsValid: true }) return;

            try
            {
                var sprite = await imageData.Image.LoadAsync();
                
                if (sprite == null) return;
            
                image.style.backgroundImage = new StyleBackground(sprite);
                image.style.maxHeight = 80;
                image.style.maxWidth = 100;
                image.style.minHeight = 80;
                image.style.minWidth = 80;
                image.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            }
            catch (Exception e)
            {
                // TODO: Log
            }
        }
    }
}