using System.Collections.Generic;
using System.Linq;
using Module.InteractiveEditor.Configs;
using Module.Utils;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Module.InteractiveEditor.Editor
{
    public class ImageWindow : OdinEditorWindow
    {
        [ShowInInspector, HideLabel]
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private ImageData currentImageData;

        [ShowInInspector]
        [FoldoutGroup("Image Settings")]
        [LabelText("Image Sprite")]
        [OnValueChanged(nameof(OnImageSpriteChanged))]
        [PropertySpace(5)]
        private Sprite imageSprite;

        [ShowInInspector]
        [FoldoutGroup("Censure Management")]
        [LabelText("Censure Areas")]
        [ListDrawerSettings(
            ShowIndexLabels = true,
            DraggableItems = false,
            ShowItemCount = true,
            CustomAddFunction = nameof(AddCensureArea),
            CustomRemoveElementFunction = nameof(RemoveCensureArea)
        )]
        [PropertySpace(5)]
        private List<CensureData> censures = new List<CensureData>();

        [ShowInInspector, ReadOnly]
        [FoldoutGroup("Image Info")]
        [LabelText("Image Size")]
        private Vector2 imageSize;

        // Removed MenuItem to clean up Interactive Editor menu
        // Use double-click ImageData to open
        public static void ShowWindow()
        {
            var window = GetWindow<ImageWindow>();
            window.titleContent = new GUIContent("Image Editor");
            window.Show();
        }

        public static void ShowWindow(ImageData imageData)
        {
            var window = GetWindow<ImageWindow>();
            window.titleContent = new GUIContent($"Image Editor - {imageData.Title}");
            window.InjectActivation(imageData);
            window.Show();
        }

        public static void ShowWindow(EpisodeData episode)
        {
            var window = GetWindow<ImageWindow>();
            window.titleContent = new GUIContent($"Images Manager - {episode.Title}");
            window.LoadEpisodeImages(episode);
            window.Show();
        }

        public void InjectActivation(ImageData imageData)
        {
            currentImageData = imageData;
            if (imageData != null)
            {
                titleContent = new GUIContent($"Image Editor - {imageData.Title}");
                LoadImageData();
            }
        }

        private void LoadEpisodeImages(EpisodeData episode)
        {
            if (episode?.ImageDatas != null && episode.ImageDatas.Count > 0)
            {
                InjectActivation(episode.ImageDatas.FirstOrDefault());
            }
        }

        private void LoadImageData()
        {
            if (currentImageData == null) return;

            imageSprite = currentImageData.ImageSprite;
            imageSize = currentImageData.GetFieldValue<Vector2>(ImageData.ImageSizeKey);
            
            censures.Clear();
            if (currentImageData.Censures != null)
            {
                censures.AddRange(currentImageData.Censures);
            }
        }

        private void OnImageSpriteChanged()
        {
            if (currentImageData == null) return;

            if (imageSprite != null)
            {
                currentImageData.SetFieldValue(ImageData.ImageSpriteKey, imageSprite);
                currentImageData.SetFieldValue(ImageData.FileNameKey, imageSprite.name);
                imageSize = new Vector2(imageSprite.rect.width, imageSprite.rect.height);
                currentImageData.SetFieldValue<Vector2>(ImageData.ImageSizeKey, imageSize);
                
                UpdateCensureImageSize();
            }
            else
            {
                currentImageData.SetFieldValue<Sprite>(ImageData.ImageSpriteKey, null);
                currentImageData.SetFieldValue(ImageData.FileNameKey, "");
                imageSize = Vector2.zero;
                currentImageData.SetFieldValue<Vector2>(ImageData.ImageSizeKey, imageSize);
            }

            EditorUtility.SetDirty(currentImageData);
        }

        private void UpdateCensureImageSize()
        {
            if (currentImageData?.Censures == null) return;
            
            foreach (var censure in currentImageData.Censures)
            {
                if (censure != null)
                {
                    censure.SetFieldValue<Vector2>(CensureData.ImageSizeKey, imageSize);
                }
            }
        }

        private CensureData AddCensureArea()
        {
            if (currentImageData == null)
            {
                EditorUtility.DisplayDialog("Error", "No image data selected.", "OK");
                return null;
            }

            var newCensure = new CensureData();
            
            // Set default position and size (center of image, 25% size)
            if (imageSize.x > 0 && imageSize.y > 0)
            {
                var defaultSize = new Vector2(imageSize.x * 0.25f, imageSize.y * 0.25f);
                var defaultPosition = new Vector2(
                    (imageSize.x - defaultSize.x) * 0.5f, 
                    (imageSize.y - defaultSize.y) * 0.5f
                );
                
                newCensure.SetFieldValue<Vector2>(CensureData.PositionKey, defaultPosition);
                newCensure.SetFieldValue<Vector2>(CensureData.SizeKey, defaultSize);
                newCensure.SetFieldValue<Vector2>(CensureData.ImageSizeKey, imageSize);
            }

            currentImageData.AddToList(ImageData.CensuresKey, newCensure);
            EditorUtility.SetDirty(currentImageData);

            return newCensure;
        }

        private void RemoveCensureArea(CensureData censure)
        {
            if (currentImageData == null || censure == null) return;

            if (EditorUtility.DisplayDialog("Delete Censure Area", 
                "Are you sure you want to delete this censure area?", 
                "Yes", "No"))
            {
                currentImageData.RemoveFromList(ImageData.CensuresKey, censure);
                EditorUtility.SetDirty(currentImageData);
            }
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Select Image Asset", ButtonSizes.Medium)]
        [EnableIf(nameof(HasValidImage))]
        private void SelectImageAsset()
        {
            if (imageSprite != null)
            {
                Selection.activeObject = imageSprite;
                EditorGUIUtility.PingObject(imageSprite);
            }
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Open Image Viewer", ButtonSizes.Large)]
        [EnableIf(nameof(HasValidImage))]
        private void OpenImageViewer()
        {
            if (currentImageData != null && imageSprite != null)
            {
                ImageViewerWindow.ShowWindow(currentImageData);
            }
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Auto-Size Censures", ButtonSizes.Medium)]
        [EnableIf(nameof(HasCensures))]
        private void AutoSizeCensures()
        {
            if (currentImageData?.Censures == null || imageSize == Vector2.zero) return;

            foreach (var censure in currentImageData.Censures)
            {
                if (censure != null)
                {
                    // Set to 20% of image size as default
                    var defaultSize = imageSize * 0.2f;
                    censure.SetFieldValue<Vector2>(CensureData.SizeKey, defaultSize);
                }
            }
            
            LoadImageData(); // Refresh display
            EditorUtility.SetDirty(currentImageData);
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Clear All Censures", ButtonSizes.Medium)]
        [EnableIf(nameof(HasCensures))]
        private void ClearAllCensures()
        {
            if (EditorUtility.DisplayDialog("Clear All Censures", 
                "Are you sure you want to remove all censure areas?", 
                "Yes", "No"))
            {
                censures.Clear();
                currentImageData.SetFieldValue<List<CensureData>>(ImageData.CensuresKey, new List<CensureData>());
                EditorUtility.SetDirty(currentImageData);
            }
        }

        [FoldoutGroup("Quick Actions")]
        [Button("Save Changes", ButtonSizes.Medium)]
        private void SaveChanges()
        {
            if (currentImageData != null)
            {
                // Sync the censures list back to the image data
                currentImageData.SetFieldValue<List<CensureData>>(ImageData.CensuresKey, censures);
                EditorUtility.SetDirty(currentImageData);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Saved", "Image data has been saved successfully.", "OK");
            }
        }

        private bool HasValidImage()
        {
            return imageSprite != null;
        }

        private bool HasCensures()
        {
            return censures.Count > 0;
        }

        protected override void OnImGUI()
        {
            if (currentImageData == null)
            {
                EditorGUILayout.HelpBox("No image data selected. Use InjectActivation to set image data or open through Episodes Window.", MessageType.Info);
                
                if (GUILayout.Button("Open Episodes Manager"))
                {
                    EpisodesManager.ShowWindow();
                }
                return;
            }

            base.OnImGUI();
        }

        protected override void OnDestroy()
        {
            if (currentImageData != null)
            {
                // Sync changes before closing
                currentImageData.SetFieldValue<List<CensureData>>(ImageData.CensuresKey, censures);
                EditorUtility.SetDirty(currentImageData);
            }
            base.OnDestroy();
        }
    }
}