﻿﻿using System;
using System.Collections.Generic;
using Module.InteractiveEditor.Runtime;
using Module.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Configs
{
    public class AnswerChoiceDialogueNode : BaseNode<DialogueAnswerExecutor>
    {
        [FoldoutGroup("Answer Settings")]
        [LabelText("Answer Text")]
        [SerializeField] private LocalizedString answerText;

        #region Editor

        public const string AnswerTextKey = nameof(answerText);

        #endregion
        
        public LocalizedString AnswerText => this.answerText;

#if !UNITY_WEBGL
        public override IReadOnlyCollection<IAddressableAsset> GetAssets()
        {
            return new List<IAddressableAsset>(0);
        }
#endif

        public override object Clone()
        {
            var item = base.Clone();
            
            item.SetFieldValue(AnswerTextKey, answerText);
            
            return item;
        }
    }

    public class AnswerChoiceDialogueNode<T> : AnswerChoiceDialogueNode
        where T : INodeExecutor
    {
        public override Type GetExecutorType()
        {
            return typeof(T);
        }
    }
}