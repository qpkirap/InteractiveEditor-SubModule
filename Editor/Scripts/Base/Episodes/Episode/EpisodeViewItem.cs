using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Module.InteractiveEditor.Configs;
using Module.Utils;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class EpisodeViewItem : Button
    {
        private VisualElement imageElement;
        public EpisodeData EpisodeData { get; private set; }
        
        public EpisodeViewItem()
        {
            AddToClassList("episode-item");
        }
        
        public void InjectData(EpisodeData episodeData)
        {
            this.EpisodeData = episodeData;
            
            text = episodeData.Title;

            var images = episodeData.GetFieldValue<List<ImageData>>(EpisodeData.ImageDatasKey);

            if (images != null)
            {
                var first = images.FirstOrDefault(x=> x.ImageSprite != null);
                
                if (first != default)
                {
                    CreateImageElementAsync(first);
                }
            }
        }

        private void CreateImageElementAsync(ImageData imageData)
        {
            if (imageData == null) return;
            
            imageElement = new VisualElement();
            
            imageElement.AddToClassList("episode-image");
            
            Add(imageElement);
            
            if (imageData.ImageSprite == null) return;

            try
            {
                var sprite = imageData.ImageSprite;
                
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