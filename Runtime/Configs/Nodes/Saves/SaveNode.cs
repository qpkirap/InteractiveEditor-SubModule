using System.Collections.Generic;
using Module.InteractiveEditor.Configs;
using UnityEngine.AddressableAssets;

namespace Module.InteractiveEditor.Runtime
{
    public class SaveNode : BaseNode<SaveNodeExecutor>
    {
#if !UNITY_WEBGL
        public override IReadOnlyCollection<IAddressableAsset> GetAssets()
        {
            return new List<IAddressableAsset>(0);
        }
#endif
        
    }
}