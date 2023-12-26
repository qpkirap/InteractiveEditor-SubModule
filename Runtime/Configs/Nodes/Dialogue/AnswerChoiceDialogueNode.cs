using System.Collections.Generic;
using Module.InteractiveEditor.Runtime;
using Module.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Configs
{
    public class AnswerChoiceDialogueNode : BaseNode<DialogueAnswerExecutor>
    {
        [SerializeField] private LocalizedString answerText;

        #region Editor

        public const string AnswerTextKey = nameof(answerText);

        #endregion
        
        public LocalizedString AnswerText => this.answerText;

        public override IReadOnlyCollection<IAddressableAsset> GetAssets()
        {
            return new List<IAddressableAsset>(0);
        }

        public override object Clone()
        {
            var item = base.Clone();
            
            item.SetFieldValue(AnswerTextKey, answerText);
            
            return item;
        }
    }
}