using System.Collections.Generic;
using Module.InteractiveEditor.Configs;
using Module.Utils;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Module.InteractiveEditor.Editor
{
    public class ImageViewerWindow : OdinEditorWindow
    {
        [ShowInInspector, ReadOnly]
        [LabelText("Current Image")]
        private ImageData currentImageData;

        [ShowInInspector, ReadOnly]
        [LabelText("Image Size")]
        private Vector2 imageSize;

        private Rect imageRect;
        private bool isDragging = false;
        private Vector2 dragStart;
        private CensureData tempCensure;

        public static void ShowWindow(ImageData imageData)
        {
            var window = GetWindow<ImageViewerWindow>();
            window.titleContent = new GUIContent($"Image Viewer - {imageData.Title}");
            window.currentImageData = imageData;
            window.LoadImageData();
            window.Show();
        }

        private void LoadImageData()
        {
            if (currentImageData?.ImageSprite != null)
            {
                imageSize = currentImageData.GetFieldValue<Vector2>(ImageData.ImageSizeKey);
            }
        }

        [FoldoutGroup("Instructions")]
        [InfoBox("Left click and drag to create censure areas.\nRight click on existing areas to remove them.\nUse the Image Window for precise editing.")]
        [Button("Open Image Window", ButtonSizes.Medium)]
        private void OpenImageWindow()
        {
            if (currentImageData != null)
            {
                ImageWindow.ShowWindow(currentImageData);
            }
        }

        protected override void OnImGUI()
        {
            base.OnImGUI();

            if (currentImageData?.ImageSprite == null)
            {
                EditorGUILayout.HelpBox("No valid image to display.", MessageType.Warning);
                return;
            }

            DrawImageArea();
            HandleMouseEvents();
        }

        private void DrawImageArea()
        {
            var sprite = currentImageData.ImageSprite;
            if (sprite == null) return;

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Visual Editor", EditorStyles.boldLabel);
            
            // Calculate display size while maintaining aspect ratio
            var maxDisplaySize = new Vector2(400f, 300f);
            var aspectRatio = sprite.rect.width / sprite.rect.height;
            
            Vector2 displaySize;
            if (aspectRatio > maxDisplaySize.x / maxDisplaySize.y)
            {
                displaySize = new Vector2(maxDisplaySize.x, maxDisplaySize.x / aspectRatio);
            }
            else
            {
                displaySize = new Vector2(maxDisplaySize.y * aspectRatio, maxDisplaySize.y);
            }

            var lastRect = GUILayoutUtility.GetLastRect();
            imageRect = new Rect(lastRect.x + 10, lastRect.y + 25, displaySize.x, displaySize.y);
            
            // Reserve space
            GUILayoutUtility.GetRect(displaySize.x, displaySize.y);

            // Draw the image
            GUI.DrawTexture(imageRect, sprite.texture);

            // Draw censure areas
            DrawCensureAreas();

            // Instructions
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox(
                "• Left click and drag to create new censure areas\n" +
                "• Right click on censure areas to delete them\n" +
                "• Use the Image Window for precise position/size editing",
                MessageType.Info);
        }

        private void DrawCensureAreas()
        {
            if (currentImageData?.Censures == null) return;

            var scaleX = imageRect.width / imageSize.x;
            var scaleY = imageRect.height / imageSize.y;

            foreach (var censure in currentImageData.Censures)
            {
                if (censure == null) continue;

                var position = censure.GetFieldValue<Vector2>(CensureData.PositionKey);
                var size = censure.GetFieldValue<Vector2>(CensureData.SizeKey);

                var displayRect = new Rect(
                    imageRect.x + position.x * scaleX,
                    imageRect.y + position.y * scaleY,
                    size.x * scaleX,
                    size.y * scaleY
                );

                // Draw censure area
                EditorGUI.DrawRect(displayRect, new Color(1, 0, 0, 0.3f));
                
                // Draw border
                GUI.Box(displayRect, "", GUI.skin.box);
            }

            // Draw temporary censure while dragging
            if (isDragging && tempCensure != null)
            {
                var position = tempCensure.GetFieldValue<Vector2>(CensureData.PositionKey);
                var size = tempCensure.GetFieldValue<Vector2>(CensureData.SizeKey);

                var displayRect = new Rect(
                    imageRect.x + position.x * scaleX,
                    imageRect.y + position.y * scaleY,
                    size.x * scaleX,
                    size.y * scaleY
                );

                EditorGUI.DrawRect(displayRect, new Color(1, 1, 0, 0.3f)); // Yellow for temp
            }
        }

        private void HandleMouseEvents()
        {
            var currentEvent = Event.current;
            if (!imageRect.Contains(currentEvent.mousePosition)) return;

            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    HandleMouseDown(currentEvent);
                    break;
                    
                case EventType.MouseDrag:
                    HandleMouseDrag(currentEvent);
                    break;
                    
                case EventType.MouseUp:
                    HandleMouseUp(currentEvent);
                    break;
            }
        }

        private void HandleMouseDown(Event evt)
        {
            if (evt.button == 0) // Left click
            {
                isDragging = true;
                dragStart = GetImageSpacePosition(evt.mousePosition);
                
                tempCensure = new CensureData();
                tempCensure.SetFieldValue<Vector2>(CensureData.PositionKey, dragStart);
                tempCensure.SetFieldValue<Vector2>(CensureData.SizeKey, Vector2.zero);
                tempCensure.SetFieldValue<Vector2>(CensureData.ImageSizeKey, imageSize);
                
                evt.Use();
                Repaint();
            }
            else if (evt.button == 1) // Right click
            {
                HandleRightClick(evt.mousePosition);
                evt.Use();
            }
        }

        private void HandleMouseDrag(Event evt)
        {
            if (isDragging && tempCensure != null)
            {
                var currentPos = GetImageSpacePosition(evt.mousePosition);
                var size = new Vector2(
                    Mathf.Abs(currentPos.x - dragStart.x),
                    Mathf.Abs(currentPos.y - dragStart.y)
                );
                var position = new Vector2(
                    Mathf.Min(dragStart.x, currentPos.x),
                    Mathf.Min(dragStart.y, currentPos.y)
                );

                tempCensure.SetFieldValue<Vector2>(CensureData.PositionKey, position);
                tempCensure.SetFieldValue<Vector2>(CensureData.SizeKey, size);
                
                Repaint();
            }
        }

        private void HandleMouseUp(Event evt)
        {
            if (isDragging && tempCensure != null)
            {
                var size = tempCensure.GetFieldValue<Vector2>(CensureData.SizeKey);
                
                // Only add if the size is reasonable (at least 5x5 pixels)
                if (size.x >= 5 && size.y >= 5)
                {
                    currentImageData.AddToList(ImageData.CensuresKey, tempCensure);
                    EditorUtility.SetDirty(currentImageData);
                }

                isDragging = false;
                tempCensure = null;
                Repaint();
            }
        }

        private void HandleRightClick(Vector2 screenPosition)
        {
            if (currentImageData?.Censures == null) return;

            var imagePos = GetImageSpacePosition(screenPosition);

            // Find censure at this position
            for (int i = currentImageData.Censures.Count - 1; i >= 0; i--)
            {
                var censure = currentImageData.Censures[i];
                if (censure == null) continue;

                var position = censure.GetFieldValue<Vector2>(CensureData.PositionKey);
                var size = censure.GetFieldValue<Vector2>(CensureData.SizeKey);
                var rect = new Rect(position.x, position.y, size.x, size.y);

                if (rect.Contains(imagePos))
                {
                    if (EditorUtility.DisplayDialog("Delete Censure", 
                        "Delete this censure area?", "Yes", "No"))
                    {
                        currentImageData.RemoveFromList(ImageData.CensuresKey, censure);
                        EditorUtility.SetDirty(currentImageData);
                        Repaint();
                    }
                    break;
                }
            }
        }

        private Vector2 GetImageSpacePosition(Vector2 screenPosition)
        {
            var relativePos = screenPosition - new Vector2(imageRect.x, imageRect.y);
            var scaleX = imageSize.x / imageRect.width;
            var scaleY = imageSize.y / imageRect.height;
            
            return new Vector2(
                Mathf.Clamp(relativePos.x * scaleX, 0, imageSize.x),
                Mathf.Clamp(relativePos.y * scaleY, 0, imageSize.y)
            );
        }
    }
}