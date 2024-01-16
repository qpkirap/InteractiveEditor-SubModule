using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Module.InteractiveEditor.Runtime
{
    public class ChoiceItem : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text text;
        
        private readonly CancellationTokenHandler token = new();
        private int index;
        
        public IObservable<Unit> OnClickAsObservable => button.OnClickAsObservable();

        public void InjectData(int index, LocalizedString localizedString)
        {
            this.index = index;
            SetText(localizedString);
        }
        
        public void Disable()
        {
            token.CancelOperation();
            
            index = -1;
        }

        private async UniTask SetText(LocalizedString localizedString)
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