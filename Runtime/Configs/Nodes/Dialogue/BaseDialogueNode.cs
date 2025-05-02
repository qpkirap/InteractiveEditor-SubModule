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
        
        public LocalizedString Dialogue => dialogue;
        
        public IReadOnlyList<ImageData> ImageData => imageDatas;
        
        public ImageData GetRandomData => imageDatas.RandomItem();

#if !UNITY_WEBGL
        public IReadOnlyList<ImageData> AddressableSprites =>
            spritesCache ??= imageDatas != default 
                ? imageDatas.Where(x=> x != default && !string.IsNullOrEmpty(x?.Image?.AssetGUID) ? x : null).Where(x=> x != default).ToList() : new List<ImageData>(0);
        
        public ImageData RandomAddressableImage => AddressableSprites.RandomItem();
        
        public override IReadOnlyCollection<IAddressableAsset> GetAssets()
        {
            try
            {
                addressableAssets ??= AddressableSprites.Select(x=> x.Image).ToList();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
            
            return addressableAssets;
        }
#endif
        
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