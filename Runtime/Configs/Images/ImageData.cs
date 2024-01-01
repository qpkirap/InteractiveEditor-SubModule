using System;
using System.Collections.Generic;
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

        #region Editor

        public const string ImageKey = nameof(image);
        public const string ImageCacheKey = nameof(imageCache);
        public const string CensuresKey = nameof(censures);

        #endregion

        private AddressableSprite imageCache;

        public AddressableSprite Image => imageCache == null || string.IsNullOrEmpty(imageCache.AssetGUID) ?
            imageCache = image != null ? new AddressableSprite(image) : default
            : imageCache;
        
        public List<CensureData> Censures => censures;
    }
}