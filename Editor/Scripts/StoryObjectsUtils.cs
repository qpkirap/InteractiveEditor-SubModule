using System.Collections.Generic;
using System.Linq;
using Module.InteractiveEditor.Configs;
using Module.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Module.InteractiveEditor
{
    public static class StoryObjectsUtils
    {
        [MenuItem("InteractiveEditor/Utils/Reference Image Copy Sprite")]
        public static void ReferenceImageCopySprite()
        {
            var storyObjects = AssetDatabase.FindAssets("t:StoryObject")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath<StoryObject>(path))
                .Where(x => x != null);

            foreach (var storyObject in storyObjects)
            {
                var episodes = storyObject.GetFieldValue<List<EpisodeData>>(StoryObject.EpisodeDatasKey);

                foreach (var episodeData in episodes)
                {
                    var imageDatas = episodeData.GetFieldValue<List<ImageData>>(EpisodeData.ImageDatasKey);
                    
                    foreach (var imageData in imageDatas)
                    {
                        var asset = imageData.GetFieldValue<AssetReference>(ImageData.ImageKey);
                        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(asset.AssetGUID));
                        imageData.SetFieldValue(ImageData.ImageSpriteKey, sprite);
                    }
                }
            }
        }
    }
}