using System.Linq;
using Module.InteractiveEditor.Configs;
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
    }
}