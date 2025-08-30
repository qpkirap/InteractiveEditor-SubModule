using DepedencyInjection;
using Managers.Router.Routs;
using UnityEngine;

namespace Managers.Router
{
    public abstract class UICanvas : RoutContainer
    {
        protected LazyInject<IRouter> router = new();
        
        protected virtual void GoBack()
        {
            if (router.Value.CurrentRouts is { Count: > 0 })
            {
                router.Value.GoBack();
            }
        }
        
        protected bool IsVisibleOnCanvas(RectTransform canvas, RectTransform uiElement)
        {
            var canvasRectTransform = canvas.GetComponent<RectTransform>();
            var corners = new Vector3[4];
            uiElement.GetWorldCorners(corners);

            for (int i = 0; i < 4; i++)
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(canvasRectTransform, corners[i], null))
                {
                    return false;
                }
            }

            return true;
        }
    }
}