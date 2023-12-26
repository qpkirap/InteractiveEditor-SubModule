using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Module.InteractiveEditor.Configs;
using UnityEngine.AddressableAssets;

namespace Module.InteractiveEditor.Runtime
{
    public class PreloadManager
    {
        private const int CacheSize = 12;
        
        private readonly Dictionary<string, (long index, UniTaskCompletionSource<IAddressableAsset>)> nodeAssets = new();
        
        public void PrepareNode(BaseNode current) 
        {
            if (current == null || nodeAssets.ContainsKey(current.Id)) return;

            var assets = current.GetAssets();
            var childrenAsset = current.ChildrenNodes.SelectMany(x => x.GetAssets());

            var allAssets = assets.Concat(childrenAsset).ToArray();

            foreach (var asset in allAssets)
            {
                if (asset == null || nodeAssets.ContainsKey(asset.AssetGUID)) continue;
                
                var complete = new UniTaskCompletionSource<IAddressableAsset>();
            
                nodeAssets.Add(asset.AssetGUID, (nodeAssets.Any() ? nodeAssets.Values.Max(x => x.index) : nodeAssets.Count, complete));
                
                LoadAssetAsync(asset).ContinueWith(() => complete.TrySetResult(asset));
            }
            
            UpdateUnload();
        }

        private void UpdateUnload()
        {
            if (nodeAssets.Count > CacheSize)
            {
                var removeAssets = nodeAssets
                    .OrderBy(x => x.Value.index)
                    .Take(nodeAssets.Count - CacheSize)
                    .ToArray();
                
                foreach (var removeAsset in removeAssets)
                {
                    UnloadAssetAsync(removeAsset.Value.Item2).Forget();
                    
                    nodeAssets.Remove(removeAsset.Key);
                }
            }
        }

        private async UniTask UnloadAssetAsync(UniTaskCompletionSource<IAddressableAsset> asset)
        {
            if (asset == null) return;

            var item = await asset.Task;
            
            if (item == null) return;
            
            await item.Release();
        }

        private async UniTask LoadAssetAsync(IAddressableAsset asset)
        {
            if (asset == null) return;

            await asset.PreloadAsync();
        }
    }
}