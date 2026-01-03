using Managers.Router.Routs;
using UnityEngine;
using VContainer;

namespace Managers.Router
{
    public abstract class UICanvas : RoutContainer
    {
        [Inject] protected IRouter router;
        
        protected virtual void GoBack()
        {
            if (router.CurrentRouts is { Count: > 0 })
            {
                router.GoBack();
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