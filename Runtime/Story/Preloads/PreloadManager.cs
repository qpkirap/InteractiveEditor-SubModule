using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Module.InteractiveEditor.Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Module.InteractiveEditor.Runtime
{
    public class PreloadManager
    {
        private const int CacheSize = 5;

        private readonly Dictionary<string, Dictionary<int, (HashSet<string> idAssets, HashSet<BaseNode> baseNodes)>> storyTreeCache = new(); //id story, depth, assets, nodes
        private readonly Dictionary<string, Dictionary<int, HashSet<IAddressableAsset>>> loadedAssets = new(); //id story, depth, assets

        public async UniTask UnloadAllAssets()
        {
            var loadedAssets = this.loadedAssets.Values.SelectMany(x => x.Values).ToList();
            
            this.loadedAssets.Clear();

            for (var i = 0; i < loadedAssets.Count; i++)
            {
                var assets = loadedAssets[i];

                await assets.ToUniTaskAsyncEnumerable().ForEachAwaitAsync(async item =>
                {
                    await item.Release();
                });
            }
            
            storyTreeCache.Clear();
        }
        
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
                
                foreach (var baseNode in storyTreeCache[obj.Id][i].baseNodes)
                {
                    Debug.Log($"Load {baseNode.Id}");
                }
                
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

        private async UniTask LoadAssetAsync(IAddressableAsset asset)
        {
            if (asset == null) return;

            await asset.PreloadAsync();
        }

        private Dictionary<int, HashSet<BaseNode>> TraverseTree(BaseNode root, int maxDepth = int.MaxValue)
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