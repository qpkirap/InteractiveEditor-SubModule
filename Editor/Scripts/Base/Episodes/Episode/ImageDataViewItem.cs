using System;
using Module.InteractiveEditor.Configs;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class ImageDataViewItem : VisualElement
    {
        private VisualElement image;
        private Label label;
        
        public ImageData ImageData { get; private set; }
        
        public ImageDataViewItem()
        {
            AddToClassList("image-data-item");
            
            image = new VisualElement();
            label = new Label();
            
            image.AddToClassList("episode-image");
            image.style.width = 0;
            image.style.height = 0;
            
            this.style.flexDirection = FlexDirection.Row;
            this.style.justifyContent = Justify.SpaceBetween;
            
            Add(image);
            Add(label);
        }

        public void InjectData(ImageData imageData)
        {
            this.ImageData = imageData;
            
            label.text = imageData.Title;

            CreateImageElement(imageData);
        }
        
        private void CreateImageElement(ImageData imageData)
        {
            if (imageData == null) return;
            
            if (imageData.ImageSprite == null) return;

            try
            {
                var sprite = imageData.ImageSprite;
                
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