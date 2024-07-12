using System;
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
        [field: SerializeField, SerializeReference, ListDrawerSettings(Expanded = true), OnValueChanged(nameof(OnGenerateId))] public List<IConditionComponent> Conditions = new();

        public override IReadOnlyCollection<IAddressableAsset> GetAssets()
        {
            return new List<IAddressableAsset>(0);
        }

        private void OnGenerateId()
        {
            Conditions.ForEach(item => item.GenerateId());
        }
    }
}