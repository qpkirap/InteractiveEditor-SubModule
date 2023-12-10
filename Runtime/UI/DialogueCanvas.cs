using Cysharp.Threading.Tasks;
using Module.InteractiveEditor.Runtime;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.UI.Story
{
    public class DialogueCanvas : UICanvas<BaseDialogueViewExecutor>
    {
        [SerializeField] private ImageController bgImage;

        public override async UniTask Init()
        {
            await base.Init();
            
            await bgImage.Init();
        }

        protected override void OnHide()
        {
            base.OnHide();
            
            bgImage.Disable();
        }

        public void SetImage(AddressableSprite sprite)
        {
            if (sprite is not { RuntimeKeyIsValid: true }) return;

            bgImage.SetImage(sprite);
        }
    }
}