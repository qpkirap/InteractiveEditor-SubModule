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
        [SerializeField] private List<CensureData> censures;
        [SerializeField] private Vector2 imageSize;
        
        [SerializeField] private Sprite imageSprite;
        
        public Sprite ImageSprite => imageSprite;
        
        public IReadOnlyList<CensureData> Censures => censures;

        #region Editor
        
        
        public const string ImageSizeKey = nameof(imageSize);
        public const string CensuresKey = nameof(censures);
        
#if UNITY_EDITOR
        [SerializeField] private string fileName; //чтобы потерять файл при изменении разрешения
        
        public const string ImageSpriteKey = nameof(imageSprite);
        public const string FileNameKey = nameof(fileName);
        
        public string FileName => fileName;

#endif

        #endregion

#if UNITY_EDITOR
        public override object Clone()
        {
                var item = base.Clone();
            
                item.SetFieldValue(ImageSpriteKey, imageSprite);

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

#endif
    }
}