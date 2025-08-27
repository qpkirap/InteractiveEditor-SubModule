﻿using System;
using System.Collections.Generic;
using Module.InteractiveEditor.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Module.InteractiveEditor.Configs
{
    [Serializable]
    public class ConditionsNode : BaseNode<ConditionsNodeExecutor>
    {
        [FoldoutGroup("Conditions")]
        [LabelText("Condition Components")]
        [field: SerializeField, SerializeReference, ListDrawerSettings(Expanded = true), OnValueChanged(nameof(OnGenerateId))] 
        public List<IConditionComponent> Conditions = new();

#if !UNITY_WEBGL
        public override IReadOnlyCollection<IAddressableAsset> GetAssets()
        {
            return new List<IAddressableAsset>(0);
        }
#endif
        

        private void OnGenerateId()
        {
            Conditions.ForEach(item => item.GenerateId());
        }
    }
}