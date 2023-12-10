using Cysharp.Threading.Tasks;
using Managers.Router;
using Module.InteractiveEditor.Runtime;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Game.UI.Story
{
    public class DialogueCanvas : UICanvas<BaseDialogueViewExecutor>
    {
        [SerializeField] private ImageController bgImage;
        [SerializeField] private TextController textController;
        [SerializeField] private Button nextButton;

        public Subject<UICanvas> OnNextButtonPressed { get; } = new Subject<UICanvas>();


        public override async UniTask Init()
        {
            await base.Init();
            
            await bgImage.Init();
            
            if (disp.Count > 0) disp.Clear();

            nextButton.OnClickAsObservable().Subscribe(_ => OnNextButtonPressed.OnNext(this)).AddTo(disp);
        }

        protected override void OnHide()
        {
            base.OnHide();
            
            bgImage.Disable();
        }

        public void SetImage(AddressableSprite sprite)
        {
            bgImage.SetImage(sprite);
        }
        
        public void SetText(LocalizedString localizedString)
        {
            textController.SetText(localizedString);
        }
    }
}