using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Module.InteractiveEditor.Runtime
{
    public class TextController : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Button hideButton;
        [SerializeField] private CanvasGroup canvasGroup;

        private readonly CancellationTokenHandler token = new();
        private readonly CompositeDisposable disp = new();

        private Sequence sequence;

        public async UniTask Init()
        {
            if (disp.Count > 0)
            {
                Disable();
            }
            
            hideButton.OnClickAsObservable().Subscribe(_ => HideText()).AddTo(disp);
        }

        private void HideText()
        {
            sequence?.Complete(true);
            
            sequence = DOTween.Sequence();

            sequence.Append(canvasGroup.alpha >= 1f ? canvasGroup.DOFade(0.05f, 0.5f) : canvasGroup.DOFade(1f, 0.5f));

            sequence.Play();
        }

        public void Disable()
        {
            sequence?.Complete(true);
            
            canvasGroup.alpha = 1f;
            
            disp.Clear();
        }

        public async UniTask SetText(LocalizedString localizedString)
        {
            if (text == null) return;
            
            token.CancelOperation();

            if (localizedString.IsEmpty)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);

                try
                {
                    text.text = await localizedString.GetLocalizedStringAsync(token.Token);

                    if (token.Token.IsCancellationRequested)
                    {
                        text.text = string.Empty;
                    }
                }
                catch (Exception e)
                {
                    //ignore
                }
            }
        }

        private void OnDestroy()
        {
            disp?.Dispose();
        }
    }
}