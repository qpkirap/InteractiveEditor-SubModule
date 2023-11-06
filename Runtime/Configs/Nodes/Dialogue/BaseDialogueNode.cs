using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Configs
{
    public class BaseDialogueNode : BaseNode
    {
        [SerializeField] private List<AssetReference> images = new();
        [SerializeField] private LocalizedString dialogue;

        #region Editor

        private const string DialogueKey = nameof(dialogue);
        public const string ImagesKey = nameof(images);

        #endregion
        
        public override NodeType NodeType => NodeType.Dialogue;
        
        protected override ExecuteResult ExecuteTask()
        {
            if (dialogue is { IsEmpty: false }) dialogue.GetLocalizedString();
            
            return ExecuteResult.SuccessState;
        }

        protected override ExecuteResult CancelTask()
        {
            return ExecuteResult.SuccessState;
        }

        public override object Clone()
        {
            var item =  base.Clone();
            
            item.SetFieldValue(DialogueKey, dialogue);
            item.SetFieldValue(ImagesKey, images);

            return item;
        }
    }
}