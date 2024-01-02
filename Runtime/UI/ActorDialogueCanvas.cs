using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Managers.Router;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Runtime;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Game.UI.Story
{
    public class ActorDialogueCanvas : UICanvas<ActorDialogueViewExecutor>
    {
        [SerializeField] private TextController actorNameController;
        [SerializeField] private List<ImageController> bgImages;
        [SerializeField] private TextController textController;
        [SerializeField] private Button nextButton;

        public Subject<UICanvas> OnNextButtonPressed { get; } = new Subject<UICanvas>();


        public override async UniTask Init()
        {
            await base.Init();

            await bgImages.ToUniTaskAsyncEnumerable().ForEachAwaitAsync(async item =>
            {
                await item.Init();
            });
            
            if (disp.Count > 0) disp.Clear();

            nextButton.OnClickAsObservable().Subscribe(_ => OnNextButtonPressed.OnNext(this)).AddTo(disp);
        }

        protected override void OnHide()
        {
            base.OnHide();
            
            bgImages.ForEach(item => item.Disable());
        }

        public void SetImage(AddressableSprite sprite)
        {
            bgImages.ForEach(item => item.SetImage(sprite));
        }
        
        public void SetText(LocalizedString localizedString)
        {
            textController.SetText(localizedString);
        }

        public void SetActor(Actor actor)
        {
            actorNameController.SetText(actor != null ? actor.Name : new LocalizedString());
        }

        public void SetCensure(IReadOnlyList<CensureData> censures)
        {
            
        }
    }
}