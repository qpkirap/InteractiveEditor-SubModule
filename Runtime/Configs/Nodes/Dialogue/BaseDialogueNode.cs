﻿﻿﻿﻿﻿﻿﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using Module.InteractiveEditor.Runtime;
using Module.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Configs
{
    public class BaseDialogueNode : BaseNode<BaseDialogueExecutor>
    {
        [FoldoutGroup("Dialogue Content")]
        [LabelText("Dialogue Text")]
        [SerializeField] private LocalizedString dialogue;
        
        [FoldoutGroup("Image Data")]
        [LabelText("Images")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "title", DraggableItems = true, ShowItemCount = true)]
        [PropertySpace(5)]
        [SerializeField] private List<ImageData> imageDatas = new();
        
#if UNITY_EDITOR
        [FoldoutGroup("Image Data")]
        [Button("Add Image from Episodes", ButtonSizes.Medium)]
        private void AddImageFromEpisodes()
        {
            var availableImages = GetAvailableEpisodeImages();
            if (availableImages.Count == 0)
            {
                UnityEngine.Debug.LogWarning("No episodes or images found in the current StoryObject");
                return;
            }
            
            // Show a simple menu for image selection
            var menu = new UnityEditor.GenericMenu();
            
            foreach (var kvp in availableImages)
            {
                var displayName = kvp.Key;
                var imageData = kvp.Value;
                
                if (imageData != null && !imageDatas.Contains(imageData))
                {
                    menu.AddItem(new UnityEngine.GUIContent(displayName), false, () => {
                        imageDatas.Add(imageData);
                        UnityEditor.EditorUtility.SetDirty(this);
                    });
                }
                else if (imageData != null)
                {
                    menu.AddDisabledItem(new UnityEngine.GUIContent(displayName + " (already added)"));
                }
            }
            
            if (menu.GetItemCount() == 0)
            {
                menu.AddDisabledItem(new UnityEngine.GUIContent("No new images to add"));
            }
            
            menu.ShowAsContext();
        }
        
        private Dictionary<string, ImageData> GetAvailableEpisodeImages()
        {
            var availableImages = new Dictionary<string, ImageData>();
            
            var storyObject = FindParentStoryObject();
            if (storyObject == null)
            {
                return availableImages;
            }
            
            var episodes = storyObject.Episodes;
            if (episodes == null || episodes.Count == 0)
            {
                return availableImages;
            }
            
            foreach (var episode in episodes)
            {
                if (episode?.ImageDatas == null) continue;
                
                var episodeTitle = !string.IsNullOrEmpty(episode.Title) ? episode.Title : $"Episode {episode.Id}";
                
                foreach (var imageData in episode.ImageDatas)
                {
                    if (imageData?.ImageSprite != null)
                    {
                        var imageName = !string.IsNullOrEmpty(imageData.Title) ? imageData.Title : imageData.ImageSprite.name;
                        var displayName = $"{episodeTitle} / {imageName}";
                        availableImages[displayName] = imageData;
                    }
                }
            }
            
            return availableImages;
        }
        
        protected StoryObject FindParentStoryObject()
        {
            // Try to get from selection
            var selectedStory = UnityEditor.Selection.activeObject as StoryObject;
            if (selectedStory != null)
            {
                return selectedStory;
            }
            
            // Try to find in assets that contain this node
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(StoryObject)}");
            foreach (var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var story = UnityEditor.AssetDatabase.LoadAssetAtPath<StoryObject>(path);
                
                if (story?.Nodes != null && story.Nodes.Contains(this))
                {
                    return story;
                }
            }
            
            return null;
        }
        
        [FoldoutGroup("Image Data")]
        [Button("Clear All Images", ButtonSizes.Small)]
        private void ClearAllImages()
        {
            if (UnityEditor.EditorUtility.DisplayDialog("Clear Images", "Are you sure you want to clear all images?", "Yes", "No"))
            {
                imageDatas.Clear();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif

        #region Editor

        private const string DialogueKey = nameof(dialogue);
        public const string ImagesDataKey = nameof(imageDatas);

        #endregion

        [NonSerialized] private IReadOnlyList<ImageData> spritesCache;
        [NonSerialized] private IReadOnlyList<IAddressableAsset> addressableAssets;
        
        public LocalizedString Dialogue => dialogue;
        
        public IReadOnlyList<ImageData> ImageData => imageDatas;
        
        public ImageData GetRandomData => imageDatas.RandomItem();

#if !UNITY_WEBGL
        public IReadOnlyList<ImageData> AddressableSprites =>
            spritesCache ??= imageDatas != default 
                ? imageDatas.Where(x=> x != default && !string.IsNullOrEmpty(x?.Image?.AssetGUID) ? x : null).Where(x=> x != default).ToList() : new List<ImageData>(0);
        
        public ImageData RandomAddressableImage => AddressableSprites.RandomItem();
        
        public override IReadOnlyCollection<IAddressableAsset> GetAssets()
        {
            try
            {
                addressableAssets ??= AddressableSprites.Select(x=> x.Image).ToList();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
            
            return addressableAssets;
        }
#endif
        
        public override object Clone()
        {
            var item =  base.Clone();
            
            item.SetFieldValue(DialogueKey, dialogue);

            var imageDataClone = new List<ImageData>();
            
            if (imageDatas != null)
            {
                foreach (var image in imageDatas)
                {
                    if (image == null) continue;
                    
                    imageDataClone.Add((ImageData)image.Clone());
                }
            }

            item.SetFieldValue(ImagesDataKey, imageDataClone);

            return item;
        }
    }
    
    public class BaseDialogueNode<T> : BaseDialogueNode
        where T : INodeExecutor
    {
        public override Type GetExecutorType()
        {
            return typeof(T);
        }
    }
}