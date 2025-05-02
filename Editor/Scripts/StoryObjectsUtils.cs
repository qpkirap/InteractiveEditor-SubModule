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
                        var sprite = imageData.ImageSprite;
                        imageData.SetFieldValue(ImageData.ImageSpriteKey, sprite);
                    }
                }
            }
        }

        [MenuItem("InteractiveEditor/Utils/Reference Image Copy FileName")]
        public static void ImageFileNameSave()
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
                        var sprite = imageData.ImageSprite;
                        
                        if (sprite == null) continue;
                        
                        imageData.SetFieldValue(ImageData.FileNameKey, sprite.name);
                    }
                }
            }
        }

        [MenuItem("InteractiveEditor/Utils/Find And Set Image By FileName")]
        public static void FindAndSetImageByFileName()
        {
            var storyObjects = AssetDatabase.FindAssets("t:StoryObject")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath<StoryObject>(path))
                .Where(x => x != null);

            var allSprites = AssetDatabase.FindAssets("t:Sprite")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
                .Where(x => x != null).ToHashSet();
            
            foreach (var storyObject in storyObjects)
            {
                var episodes = storyObject.GetFieldValue<List<EpisodeData>>(StoryObject.EpisodeDatasKey);

                foreach (var episodeData in episodes)
                {
                    var imageDatas = episodeData.GetFieldValue<List<ImageData>>(EpisodeData.ImageDatasKey);
                    
                    foreach (var imageData in imageDatas)
                    {
                        if (imageData == null || string.IsNullOrEmpty(imageData.FileName)) continue;
                        
                        var fileName = imageData.FileName;
                        
                        var sprite = allSprites.FirstOrDefault(x => x.name == fileName);
                        imageData.SetFieldValue(ImageData.ImageSpriteKey, sprite);
                        
                        imageData.SetFieldValue(ImageData.ImageSizeKey, sprite != null ? new Vector2(sprite.rect.width, sprite.rect.height) : default);
                    }
                }
            }
        }
    }
}