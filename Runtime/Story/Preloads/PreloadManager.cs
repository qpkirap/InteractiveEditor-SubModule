using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Module.InteractiveEditor.Configs;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;

namespace Module.InteractiveEditor.Runtime
{
    public class PreloadManager
    {
        private const int CacheSize = 6;
        
        private readonly Dictionary<string, List<(string assetGuid, UniTaskCompletionSource<IAddressableAsset> task)>> nodeAssets = new();

        public bool IsCompletePreload => nodeAssets.Values.SelectMany(x=> x.Select(z => z.task)).All(x => x.Task.Status == UniTaskStatus.Succeeded);
        
        public void PrepareAssets(BaseNode current) 
        {
            if (current == null)
            {
                return;
            }

            var tree = TraverseTree(current);
            
            foreach (var (depth, nodes) in tree)
            {
                foreach (var baseNode in nodes)
                {
                    if (!nodeAssets.ContainsKey(baseNode.Id))
                    {
                        var currentAssets = baseNode.GetAssets();
            
                        if (currentAssets == default) return;

                        foreach (var asset in currentAssets)
                        {
                            var task = new UniTaskCompletionSource<IAddressableAsset>();

                            if (nodeAssets.TryGetValue(baseNode.Id, out var list))
                            {
                                list.Add((asset.AssetGUID, task));
                            }
                            else
                            {
                                nodeAssets.Add(baseNode.Id, new List<(string assetGuid, UniTaskCompletionSource<IAddressableAsset> task)> { (asset.AssetGUID, task) });
                            }
                    
                            Debug.Log($"load {baseNode.Id}");
                
                            LoadAssetAsync(asset).ContinueWith(() => task.TrySetResult(asset));
                        }
                    }
                }
            }
            
            UnloadAssets(tree);
        }

        private void UnloadAssets(Dictionary<int, HashSet<BaseNode>> tree)
        {
            var unloadList = new List<(string idNode, string idAsset, UniTaskCompletionSource<IAddressableAsset> asset)>();
            var idNodes = tree.Values.SelectMany(x => x.Select(z => z.Id)).ToHashSet();
            
            foreach (var (idNode, list) in nodeAssets)
            {
                if (!idNodes.Contains(idNode))
                {
                    foreach (var (idAsset, task) in list)
                    {
                        unloadList.Add((idNode, idAsset, task));
                    }
                }
            }
            
            foreach (var dataItem in unloadList)
            {
                Debug.Log($"Unload {dataItem.idNode}");
                
                UnloadAssetAsync(dataItem.asset).Forget();
                    
                nodeAssets.Remove(dataItem.idNode);
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

        public Dictionary<int, HashSet<BaseNode>> TraverseTree(BaseNode root)
        {
            var result = new Dictionary<int, HashSet<BaseNode>>();
            TraverseNode(root, 0, CacheSize, result);
            return result;
        }
        
        private void TraverseNode(BaseNode node, int depth, int maxDepth, Dictionary<int, HashSet<BaseNode>> result)
        {
            if (depth > maxDepth) return;
            
            if (!result.ContainsKey(depth))
            {
                result[depth] = new HashSet<BaseNode>();
            }
        
            result[depth].Add(node);

            foreach (var child in node.ChildrenNodes)
            {
                TraverseNode(child, depth + 1, maxDepth, result);
            }
        }
    }
}