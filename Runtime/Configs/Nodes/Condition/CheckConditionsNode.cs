using System;
using System.Collections.Generic;
using Module.InteractiveEditor.Runtime;
using UnityEngine.AddressableAssets;

namespace Module.InteractiveEditor.Configs
{
    [Serializable]
    public class CheckConditionsNode : BaseNode<CheckConditionsNodeExecutor>
    {
        public override IReadOnlyCollection<IAddressableAsset> GetAssets()
        {
            return new List<IAddressableAsset>(0);
        }
    }
}