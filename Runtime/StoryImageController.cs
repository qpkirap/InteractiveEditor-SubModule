using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Module.InteractiveEditor.Runtime
{
    public class StoryImageController : MonoBehaviour
    {
        [SerializeField] private Image image;

        private readonly CancellationTokenHandler tokenHandler = new();

        public async UniTask SetImage(AddressableSprite sprite)
        {
            if (sprite == null) return;

            var load = await sprite.LoadAsync(token: tokenHandler.Token);
            
            if (tokenHandler.Token.IsCancellationRequested || load == null) return;

            if (image != null) image.sprite = load;
        }

        public void Disable()
        {
            tokenHandler.CancelOperation();
        }
    }
}