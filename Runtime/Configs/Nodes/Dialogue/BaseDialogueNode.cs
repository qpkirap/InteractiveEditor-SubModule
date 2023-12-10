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
        [SerializeField] private List<AssetReference> images = new();
        [SerializeField] private LocalizedString dialogue;

        #region Editor

        private const string DialogueKey = nameof(dialogue);
        public const string ImagesKey = nameof(images);

        #endregion

        private IReadOnlyList<AddressableSprite> sprites;
        
        public IReadOnlyList<AddressableSprite> AddressableSprites =>
            sprites == null || sprites.Any(x => string.IsNullOrEmpty(x.AssetGUID))
                ? sprites = images.Select(asset => new AddressableSprite(asset)).ToList()
                : sprites;
        
        public AddressableSprite RandomImage => AddressableSprites.RandomItem();
        public LocalizedString Dialogue => dialogue;
        

        public override object Clone()
        {
            var item =  base.Clone();
            
            item.SetFieldValue(DialogueKey, dialogue);
            item.SetFieldValue(ImagesKey, images);

            return item;
        }
    }
    
    public class BaseDialogueNode<T> : BaseDialogueNode
        where T : INodeExecute
    {
    }
}