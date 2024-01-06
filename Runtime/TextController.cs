using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Runtime
{
    public class TextController : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        private readonly CancellationTokenHandler token = new();

        public async UniTask Init()
        {
        }

        public void Disable()
        {
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
    }
}