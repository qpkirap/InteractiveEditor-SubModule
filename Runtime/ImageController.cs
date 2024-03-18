using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Module.InteractiveEditor.Runtime
{
    public class ImageController : MonoBehaviour
    {
        [SerializeField] private Image image;

        private readonly CancellationTokenHandler tokenHandler = new();
        

        public async UniTask Init(object[] args = null)
        {
        }

        public void Disable()
        {
            tokenHandler.CancelOperation();

            image.sprite = null;
        }

        public async UniTask SetImage(AddressableSprite sprite)
        {
            Debug.Log($"Try set image {sprite}");
            
            if (sprite is not { RuntimeKeyIsValid: true })
            {
                image.sprite = null;
                
                return;
            }
            
            tokenHandler.CancelOperation();

            var load = await sprite.LoadAsync(token: tokenHandler.Token);
            
            if (tokenHandler.Token.IsCancellationRequested || load == null) return;

            if (image != null)
            {
                Debug.Log($"Set image {load.name}");
                
                image.sprite = load;
            }
        }
    }
}