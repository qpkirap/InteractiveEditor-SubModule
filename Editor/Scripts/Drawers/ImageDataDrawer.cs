using Module.InteractiveEditor.Configs;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Module.InteractiveEditor.Editor
{
    /// <summary>
    /// Custom property drawer for ImageData that displays image previews
    /// </summary>
    [OdinDrawer]
    public class ImageDataDrawer : OdinValueDrawer<ImageData>
    {
        private const float PREVIEW_SIZE = 50f;
        private const float MARGIN = 4f;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var imageData = this.ValueEntry.SmartValue;
            if (imageData == null)
            {
                EditorGUILayout.LabelField(label, new GUIContent("[Null ImageData]"));
                return;
            }

            EditorGUILayout.BeginHorizontal();

            // Draw image preview
            DrawImagePreview(imageData);

            // Draw the main property content
            EditorGUILayout.BeginVertical();
            
            // Draw title and info
            DrawImageInfo(imageData);
            
            // Draw the actual property fields using inline editor
            if (SirenixEditorGUI.BeginFadeGroup(this, imageData != null))
            {
                EditorGUILayout.Space(2);
                this.CallNextDrawer(null); // Draw the default property content
            }
            SirenixEditorGUI.EndFadeGroup();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawImagePreview(ImageData imageData)
        {
            var previewRect = GUILayoutUtility.GetRect(PREVIEW_SIZE, PREVIEW_SIZE, GUILayout.Width(PREVIEW_SIZE), GUILayout.Height(PREVIEW_SIZE));
            
            if (imageData.ImageSprite != null)
            {
                // Draw the sprite preview
                var sprite = imageData.ImageSprite;
                var texture = sprite.texture;
                
                if (texture != null)
                {
                    // Calculate UV coordinates for the sprite
                    var uvRect = new Rect(
                        sprite.rect.x / texture.width,
                        sprite.rect.y / texture.height,
                        sprite.rect.width / texture.width,
                        sprite.rect.height / texture.height
                    );
                    
                    GUI.DrawTextureWithTexCoords(previewRect, texture, uvRect);
                    
                    // Draw border around preview
                    EditorGUI.DrawRect(previewRect, new Color(0, 0, 0, 0.3f));
                    EditorGUI.DrawRect(new Rect(previewRect.x, previewRect.y, previewRect.width, 1), Color.black);
                    EditorGUI.DrawRect(new Rect(previewRect.x, previewRect.yMax - 1, previewRect.width, 1), Color.black);
                    EditorGUI.DrawRect(new Rect(previewRect.x, previewRect.y, 1, previewRect.height), Color.black);
                    EditorGUI.DrawRect(new Rect(previewRect.xMax - 1, previewRect.y, 1, previewRect.height), Color.black);
                }
            }
            else
            {
                // Draw placeholder for missing sprite
                EditorGUI.DrawRect(previewRect, new Color(0.3f, 0.3f, 0.3f, 1f));
                
                var iconRect = new Rect(
                    previewRect.x + previewRect.width * 0.25f,
                    previewRect.y + previewRect.height * 0.25f,
                    previewRect.width * 0.5f,
                    previewRect.height * 0.5f
                );
                
                GUI.Label(iconRect, EditorGUIUtility.IconContent("Texture Icon"), new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.white }
                });
                
                // Draw border
                EditorGUI.DrawRect(new Rect(previewRect.x, previewRect.y, previewRect.width, 1), Color.red);
                EditorGUI.DrawRect(new Rect(previewRect.x, previewRect.yMax - 1, previewRect.width, 1), Color.red);
                EditorGUI.DrawRect(new Rect(previewRect.x, previewRect.y, 1, previewRect.height), Color.red);
                EditorGUI.DrawRect(new Rect(previewRect.xMax - 1, previewRect.y, 1, previewRect.height), Color.red);
            }
            
            // Handle click on preview to select sprite
            if (Event.current.type == EventType.MouseDown && previewRect.Contains(Event.current.mousePosition))
            {
                if (imageData.ImageSprite != null)
                {
                    Selection.activeObject = imageData.ImageSprite;
                    EditorGUIUtility.PingObject(imageData.ImageSprite);
                }
                Event.current.Use();
            }
        }

        private void DrawImageInfo(ImageData imageData)
        {
            // Draw title
            var titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12
            };
            
            string title = !string.IsNullOrEmpty(imageData.Title) ? imageData.Title : "Untitled Image";
            EditorGUILayout.LabelField(title, titleStyle);
            
            // Draw sprite info
            if (imageData.ImageSprite != null)
            {
                var infoStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    normal = { textColor = Color.gray }
                };
                
                var sprite = imageData.ImageSprite;
                string info = $"Sprite: {sprite.name} ({sprite.rect.width}x{sprite.rect.height})";
                EditorGUILayout.LabelField(info, infoStyle);
                
                // Show censure count if any
                if (imageData.Censures != null && imageData.Censures.Count > 0)
                {
                    string censureInfo = $"Censure Areas: {imageData.Censures.Count}";
                    EditorGUILayout.LabelField(censureInfo, infoStyle);
                }
            }
            else
            {
                var warningStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    normal = { textColor = Color.red }
                };
                EditorGUILayout.LabelField("No sprite assigned", warningStyle);
            }
        }
    }
}