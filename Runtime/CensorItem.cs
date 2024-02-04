using Module.InteractiveEditor.Configs;
using UnityEngine;
using UnityEngine.UI;

namespace Module.InteractiveEditor.Runtime
{
    public class CensorItem : MonoBehaviour
    {
        [SerializeField] private RectTransform rect;
        
        [Header("Screen Mode")]
        [SerializeField] private CanvasScaler.ScreenMatchMode screenMatchMode;
        [SerializeField][Range(0, 1)] private float screenMatchWidthOrHeight;

        private CensureData data;
        private const float screenResolutionWidth = 768;
        private const float screenResolutionHeight = 512;
        
        public void InjectData(CensureData data)
        {
            this.data = data;
            
            UpdateView();
        }
        
        private void UpdateView()
        {
            if (data == null) return;

            var scaleFactor = GetImageScaleFactorSize();
            var offset = GetOffsetPosition();

            rect.sizeDelta = data.Size * scaleFactor;
            rect.anchoredPosition = (data.Position * scaleFactor + offset) * new Vector2(1, -1);
        }

        private Vector2 GetScreenSize()
        {
            var scaleFactor = 1f;

            switch (screenMatchMode)
            {
                case CanvasScaler.ScreenMatchMode.MatchWidthOrHeight:
                {
                    var logWidth = Mathf.Log(Screen.width / screenResolutionWidth, 2);
                    var logHeight = Mathf.Log(Screen.height / screenResolutionHeight, 2);
                    var logWeightedAverage = Mathf.Lerp(logWidth, logHeight, screenMatchWidthOrHeight);
                    scaleFactor = Mathf.Pow(2, logWeightedAverage);
                    
                    break;
                }
                case CanvasScaler.ScreenMatchMode.Expand:
                {
                    scaleFactor = Mathf.Min(Screen.width / screenResolutionWidth,
                        Screen.height / screenResolutionHeight);
                    break;
                }
                case CanvasScaler.ScreenMatchMode.Shrink:
                {
                    scaleFactor = Mathf.Max(Screen.width / screenResolutionWidth,
                        Screen.height / screenResolutionHeight);
                    break;
                }
            }
            
            var testWidth = scaleFactor * screenResolutionWidth;
            var testHeight = scaleFactor * screenResolutionHeight;
                    
            var offsetWidth = testWidth - Screen.width;
            var offsetHeight = testHeight - Screen.height;

            var ratioWidth = offsetWidth / testWidth;
            var ratioHeight = offsetHeight / testHeight;
            
            var calcWidth = screenResolutionWidth - ratioWidth * screenResolutionWidth;
            var calcHeight = screenResolutionHeight - ratioHeight * screenResolutionHeight;

            return new Vector2(calcWidth, calcHeight);
        }

        private Vector2 GetOffsetPosition()
        {
            var screenSize = GetScreenSize();
            var imageSizeOnScreen = GetImageResolution();

            var offsetX = 0f;
            var offsetY = 0f;

            if (imageSizeOnScreen.x > screenSize.x)
            {
                offsetX = (imageSizeOnScreen.x - screenSize.x) / 2;
            }
            else
            {
                offsetX = (screenSize.x - imageSizeOnScreen.x) / 2;
            }

            if (imageSizeOnScreen.y > screenSize.y)
            {
                offsetY = (imageSizeOnScreen.y - screenSize.y) / 2;
            }
            else
            {
                offsetY = (screenSize.y - imageSizeOnScreen.y) / 2;
            }

            return new Vector2(offsetX, offsetY);
        }

        private Vector2 GetImageScaleFactorSize()
        {
            var imageSizeOnScreen = GetImageResolution();
            
            var scaleX = imageSizeOnScreen.x / data.ImageSize.x;
            var scaleY = imageSizeOnScreen.y / data.ImageSize.y;
            
            return new Vector2(scaleX, scaleY);
        }
        
        private Vector2 GetImageResolution()
        {
            var screenSize = GetScreenSize();

            var screenAspectRatio = screenResolutionWidth / screenResolutionHeight;
            var imageAspectRatio = data.ImageSize.x / data.ImageSize.y;
        
            float finalWidth;
            float finalHeight;
        
            if (screenAspectRatio > imageAspectRatio)
            {
                finalWidth = screenSize.x;
                finalHeight = Mathf.RoundToInt(finalWidth / imageAspectRatio);
            }
            else
            {
                finalHeight = screenSize.y;
                finalWidth = Mathf.RoundToInt(finalHeight * imageAspectRatio);
            }
        
            return new Vector2(finalWidth, finalHeight);
        }
    }
}