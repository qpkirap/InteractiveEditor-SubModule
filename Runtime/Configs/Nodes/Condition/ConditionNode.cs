using System.Collections.Generic;
using Module.InteractiveEditor.Runtime;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Module.InteractiveEditor.Configs
{
    public class ConditionNode : BaseNode<ConditionNodeExecutor>
    {
        [field: SerializeField, SerializeReference] public List<ICondition> Conditions = new();
        public override IReadOnlyCollection<IAddressableAsset> GetAssets()
        {
            return new List<IAddressableAsset>(0);
        }
    }
}