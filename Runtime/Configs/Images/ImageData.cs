using System;
using System.Collections.Generic;
using Module.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Component = Module.InteractiveEditor.Runtime.Component;

namespace Module.InteractiveEditor.Configs
{
    [Serializable]
    public class ImageData : Component
    {
        [SerializeField] private AssetReference image;
        [SerializeField] private List<CensureData> censures;
        [SerializeField] private Vector2 imageSize;

        #region Editor

        public const string ImageKey = nameof(image);
        public const string ImageSizeKey = nameof(imageSize);
        public const string ImageCacheKey = nameof(imageCache);
        public const string CensuresKey = nameof(censures);

        #endregion

        private AddressableSprite imageCache;

        public AddressableSprite Image => imageCache == null || string.IsNullOrEmpty(imageCache.AssetGUID) ?
            imageCache = image != null ? new AddressableSprite(image) : default
            : imageCache;
        
        public IReadOnlyList<CensureData> Censures => censures;

        public override object Clone()
        {
            var item = base.Clone();
            
            item.SetFieldValue(ImageKey, image);

            var censureClone = new List<CensureData>();
            
            if (censures != null)
            {
                foreach (var censure in censures)
                {
                    if (censure == null) continue;
                    
                    censureClone.Add(censure.Clone());
                }
            }
            
            item.SetFieldValue(CensuresKey, censureClone);
            
            return item;
        }
    }
}