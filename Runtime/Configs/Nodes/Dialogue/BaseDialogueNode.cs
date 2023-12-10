using System.Collections.Generic;
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
        
        public AddressableSprite RandomImage => new(images.RandomItem());
        

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