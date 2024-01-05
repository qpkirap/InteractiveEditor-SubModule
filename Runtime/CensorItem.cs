using Module.InteractiveEditor.Configs;
using UnityEngine;

namespace Module.InteractiveEditor.Runtime
{
    public class CensorItem : MonoBehaviour
    {
        [SerializeField] private RectTransform rect;

        private CensureData data;
        private const int screenResolutionWidth = 768;
        private const int screenResolutionHeight = 512;
        
        public void InjectData(CensureData data)
        {
            this.data = data;
            
            UpdateView();
        }

        private void UpdateView()
        {
            if (data == null) return;

            var scaleFactor = GetScaleFactorSize();
            var offset = GetOffsetPosition();

            rect.sizeDelta = data.Size * scaleFactor;
            rect.anchoredPosition = data.Position * scaleFactor + offset;
        }

        private Vector2 GetOffsetPosition()
        {
            var imageSizeOnScreen = CalculateImageResolution();

            var offsetX = 0f;
            var offsetY = 0f;

            if (imageSizeOnScreen.x > screenResolutionWidth)
            {
                offsetX = (imageSizeOnScreen.x - screenResolutionWidth) / 2;
            }
            else
            {
                offsetX = (screenResolutionWidth - imageSizeOnScreen.x) / 2;
            }

            if (imageSizeOnScreen.y > screenResolutionHeight)
            {
                offsetY = (imageSizeOnScreen.y - screenResolutionHeight) / 2;
            }
            else
            {
                offsetY = (screenResolutionHeight - imageSizeOnScreen.y) / 2;
            }

            return new Vector2(offsetX, offsetY);
        }

        private Vector2 GetScaleFactorSize()
        {
            var imageSizeOnScreen = CalculateImageResolution();
            
            var scaleX = imageSizeOnScreen.x / data.ImageSize.x;
            var scaleY = imageSizeOnScreen.y / data.ImageSize.y;
            
            return new Vector2(scaleX, scaleY);
        }
        
        private Vector2 CalculateImageResolution()
        {
            var screenAspectRatio = (float)screenResolutionWidth / screenResolutionHeight;
            var imageAspectRatio = data.ImageSize.x / data.ImageSize.y;
        
            int finalWidth;
            int finalHeight;
        
            if (screenAspectRatio > imageAspectRatio)
            {
                finalWidth = screenResolutionHeight;
                finalHeight = Mathf.RoundToInt(finalWidth / imageAspectRatio);
            }
            else
            {
                finalHeight = screenResolutionWidth;
                finalWidth = Mathf.RoundToInt(finalHeight * imageAspectRatio);
            }
        
            return new Vector2(finalWidth, finalHeight);
        }
    }
}