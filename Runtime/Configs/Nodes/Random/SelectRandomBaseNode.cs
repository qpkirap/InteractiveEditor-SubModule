using System.Collections.Generic;
using Module.InteractiveEditor.Runtime;
using UnityEngine.AddressableAssets;

namespace Module.InteractiveEditor.Configs
{
    public class SelectRandomBaseNode : BaseNode<SelectRandomExecutor>
    {
#if !UNITY_WEBGL
        public override IReadOnlyCollection<IAddressableAsset> GetAssets()
        {
            return new List<IAddressableAsset>(0);
        }
#endif
        
    }
}