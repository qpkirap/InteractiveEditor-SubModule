using System.IO;
using System.Linq;
using Module.InteractiveEditor.Configs;
using Module.Utils;
using UnityEditor;
using UnityEngine;

namespace Module.InteractiveEditor
{
    public static class Tests
    {
        [MenuItem("InteractiveEditor/Tests/TestAssets")]
        public static void TestNodeImages()
        {
            var configs = AssetDatabase.FindAssets("t:StoryObject");
            var assetConfigs = configs.Select(x => AssetDatabase.LoadAssetAtPath<StoryObject>(AssetDatabase.GUIDToAssetPath(x))).ToList();
            
            foreach (var storyObject in assetConfigs)
            {
                foreach (var node in storyObject.Nodes)
                {
                    var assets = node.GetAssets();
                    
                    if (assets == null)
                    {
                        Debug.LogError($"Asset is null nodeTitle {node.Title}");
                    }

                    foreach (var addressableAsset in assets)
                    {
                        if (addressableAsset == null || string.IsNullOrEmpty(addressableAsset.AssetGUID))
                        {
                            Debug.LogError($"Asset is null nodeTitle {node.Title}");
                        }
                    }
                }
            }
        }
        
        [MenuItem("InteractiveEditor/Tests/TestAssetsPaths")]
        public static void TestAssetsPaths()
        {
            var configs = AssetDatabase.FindAssets("t:StoryObject");
            var assetConfigs = configs.Select(x => AssetDatabase.LoadAssetAtPath<StoryObject>(AssetDatabase.GUIDToAssetPath(x))).ToList();
            
            foreach (var storyObject in assetConfigs)
            {
                foreach (var node in storyObject.Nodes)
                {
                    var assets = node.GetAssets();
                    
                    foreach (var addressableAsset in assets)
                    {
                        if (string.IsNullOrEmpty(addressableAsset.AssetGUID)) continue;
                        
                        var assetPath = AssetDatabase.GUIDToAssetPath(addressableAsset.AssetGUID);
                        var fileName = Path.GetFileName(assetPath);
                        
                        var assetPathWithoutFileName = assetPath.Replace(fileName, "");
                        
                        Debug.Log($"{assetPathWithoutFileName}");

                        if (node is { } baseNode)
                        {
                            var desc = baseNode.GetFieldValue<string>(BaseNode.DescriptionKey);
                            
                            Debug.Log(desc);
                        }
                    }
                }
            }
        }
    }
}