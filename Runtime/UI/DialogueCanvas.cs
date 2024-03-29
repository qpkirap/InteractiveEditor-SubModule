using System.Collections.Generic;
using Cysharp.Threading.Tasks.Linq;
using DepedencyInjection;
using Managers.Router;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Runtime;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.UI;
using YG.MenuNav;

namespace Game.UI.Story
{
    public class DialogueCanvas : UICanvas<BaseDialogueViewExecutor>
    {
        [SerializeField] private List<ImageController> bgImages;
        [SerializeField] private CensorContainerController censorController;
        [SerializeField] private TextController textController;
        [SerializeField] private Button nextButton;

        private readonly LazyInject<MenuNavigation> menuNavigation = new();

        public Subject<UICanvas> OnNextButtonPressed { get; } = new Subject<UICanvas>();

        protected override void OnShow()
        {
            base.OnShow();
            
            bgImages.ToUniTaskAsyncEnumerable().ForEachAwaitAsync(async item =>
            {
                await item.Init();
            });
            
            if (disp.Count > 0) disp.Clear();

            if (nextButton != null)
                nextButton.OnClickAsObservable().Subscribe(_ => OnNextButtonPressed.OnNext(this)).AddTo(disp);
            
            textController.Init();
            
            menuNavigation.Value.SelectButton(nextButton);
        }
        
        protected override void OnHide()
        {
            base.OnHide();
            
            bgImages.ForEach(item => item.Disable());
            
            censorController.Disable();
            textController.Disable();
        }

        public void SetImage(AddressableSprite sprite)
        {
            bgImages.ForEach(item => item.SetImage(sprite));
        }
        
        public void SetText(LocalizedString localizedString)
        {
            textController.SetText(localizedString);
        }

        public void SetCensure(IReadOnlyList<CensureData> censures)
        {
            censorController.InjectData(censures);
        }
    }
}