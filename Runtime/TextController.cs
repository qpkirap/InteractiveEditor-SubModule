using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Runtime
{
    public class TextController : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        public async UniTask Init()
        {
        }

        public void Disable()
        {
        }

        public void SetText(LocalizedString localizedString)
        {
            if(text == null) return;

            if (localizedString.IsEmpty)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                text.text = localizedString.GetLocalizedString();
            }
        }
    }
}