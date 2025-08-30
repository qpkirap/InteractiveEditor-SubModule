using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Managers.Router
{
    public class LoadingScreenContainer : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = .6f;
        [SerializeField] private CanvasGroup canvasGroup;

        public virtual async UniTask Init()
        {
        }

        public async UniTask Fade(float finalAlpha, bool cancel = false)
        {
            var duration = cancel ? 0.01f : fadeDuration;

            await Fade(finalAlpha, duration);
        }

        public async UniTask Fade(float finalAlpha, float duration)
        {
            await canvasGroup
                .DOFade(finalAlpha, duration)
                .SetEase(Ease.InQuint)
                .SetUpdate(true);
        }
    }
}