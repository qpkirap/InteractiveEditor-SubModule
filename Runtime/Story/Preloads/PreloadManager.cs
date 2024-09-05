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
        private const int CacheSize = 15;

        private readonly Dictionary<string, Dictionary<int, (HashSet<string> idAssets, HashSet<BaseNode> baseNodes)>> storyTreeCache = new(); //id story, depth, assets, nodes
        private readonly Dictionary<string, Dictionary<int, HashSet<IAddressableAsset>>> loadedAssets = new(); //id story, depth, assets
        
        private readonly Dictionary<string, List<(string assetGuid, UniTaskCompletionSource<IAddressableAsset> task)>> nodeAssets = new();

        
        public async UniTask InitStory(StoryObject obj, BaseNode startNode)
        {
            var tree = TraverseTree(startNode);

            if (storyTreeCache.ContainsKey(obj.Id))
            {
                storyTreeCache.Remove(obj.Id); //вероятно нужно добавить выгрузку
            }
            
            storyTreeCache.Add(obj.Id, new Dictionary<int, (HashSet<string> idAssets, HashSet<BaseNode> baseNodes)>());
            
            foreach (var (depth, nodes) in tree)
            {
                foreach (var node in nodes)
                {
                    if (!storyTreeCache[obj.Id].ContainsKey(depth))
                    {
                        storyTreeCache[obj.Id].Add(depth, (new HashSet<string>(), new HashSet<BaseNode>()));
                    }
                    
                    var assets = node.GetAssets();
                    
                    if (assets == default) continue;
                    
                    foreach (var asset in assets)
                    {
                        storyTreeCache[obj.Id][depth].idAssets.Add(asset.AssetGUID);
                    }
                    
                    storyTreeCache[obj.Id][depth].baseNodes.Add(node);
                }
            }
            
            await PrepareAssets(0, obj);
        }

        public async UniTask PrepareAssets(int currentDepth, StoryObject obj)
        {
            var endDepth = currentDepth + CacheSize;
            if (endDepth > storyTreeCache[obj.Id].Count) endDepth = storyTreeCache[obj.Id].Count - 1;

            var loadedTask = new List<UniTask>();

            for (var i = currentDepth; i <= endDepth; i++)
            {
                if (loadedAssets.ContainsKey(obj.Id) && loadedAssets[obj.Id].ContainsKey(i)
                    || !storyTreeCache[obj.Id].ContainsKey(i)) continue;
                
                loadedAssets.TryAdd(obj.Id, new Dictionary<int, HashSet<IAddressableAsset>>());
                loadedAssets[obj.Id].Add(i, new HashSet<IAddressableAsset>());

                var assets = storyTreeCache[obj.Id][i].baseNodes.SelectMany(x => x.GetAssets());
                
                foreach (var addressableAsset in assets)
                {
                    var task = LoadAssetAsync(addressableAsset);
                    loadedTask.Add(task);
                    
                    loadedAssets[obj.Id][i].Add(addressableAsset);
                }
            }
            
            await UniTask.WhenAll(loadedTask);
            
            var unloadedAssets = new List<IAddressableAsset>();
            var loadedAssetsDic = loadedAssets[obj.Id];
            var needAssets = storyTreeCache[obj.Id]
                .Where(x => x.Key >= currentDepth)
                .SelectMany(x => x.Value.idAssets)
                .ToHashSet();

            foreach (var (depth , assets) in loadedAssetsDic)
            {
                foreach (var addressableAsset in assets)
                {
                    if (needAssets.Contains(addressableAsset.AssetGUID)) continue;
                    unloadedAssets.Add(addressableAsset);
                }
            }
            
            foreach (var asset in unloadedAssets)
            {
                asset.Release().Forget();
            }
        }
        
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

        public Dictionary<int, HashSet<BaseNode>> TraverseTree(BaseNode root, int maxDepth = int.MaxValue)
        {
            var result = new Dictionary<int, HashSet<BaseNode>>();
            TraverseNode(root, 0, maxDepth, result);
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