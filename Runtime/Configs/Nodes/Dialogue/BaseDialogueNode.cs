using System;
using System.Collections.Generic;
using System.Linq;
using Module.InteractiveEditor.Runtime;
using Module.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Configs
{
    public class BaseDialogueNode : BaseNode<BaseDialogueExecutor>
    {
        [HideInInspector][SerializeField] private List<ImageData> imageDatas = new();
        [SerializeField] private LocalizedString dialogue;

        #region Editor

        private const string DialogueKey = nameof(dialogue);
        public const string ImagesDataKey = nameof(imageDatas);

        #endregion

        [NonSerialized] private IReadOnlyList<ImageData> spritesCache;
        [NonSerialized] private IReadOnlyList<IAddressableAsset> addressableAssets;
        
        public IReadOnlyList<ImageData> AddressableSprites =>
            spritesCache == null || spritesCache.Any(x => string.IsNullOrEmpty(x.Image.AssetGUID))
                ? spritesCache = imageDatas.ToList()
                : spritesCache;
        
        public ImageData RandomImage => AddressableSprites.RandomItem();
        public LocalizedString Dialogue => dialogue;


        public override IReadOnlyCollection<IAddressableAsset> GetAssets()
        {
            addressableAssets ??= AddressableSprites.Select(x=> x.Image).ToList();
            
            return addressableAssets;
        }

        public override object Clone()
        {
            var item =  base.Clone();
            
            item.SetFieldValue(DialogueKey, dialogue);

            var imageDataClone = new List<ImageData>();
            
            if (imageDatas != null)
            {
                foreach (var image in imageDatas)
                {
                    if (image == null) continue;
                    
                    imageDataClone.Add((ImageData)image.Clone());
                }
            }

            item.SetFieldValue(ImagesDataKey, imageDataClone);

            return item;
        }
    }
    
    public class BaseDialogueNode<T> : BaseDialogueNode
        where T : INodeExecute
    {
        public override Type GetExecutorType()
        {
            return typeof(T);
        }
    }
}