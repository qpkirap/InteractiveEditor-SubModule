using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Module.InteractiveEditor.Configs;
using Module.InteractiveEditor.Runtime;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace Module.InteractiveEditor.Saves.UI.Story
{
    public abstract class DialogueSelectChoiceCanvas<T> : UICanvas<T>
        where T : IViewNodeExecute
    {
        [SerializeField] private List<ImageController> bgImages;
        [SerializeField] private CensorContainerController censorController;
        [SerializeField] private TextController textController;
        [SerializeField] private ChoiceContainer choiceContainer;

        public override async UniTask Init()
        { 
            await choiceContainer.Init();
            await textController.Init();
            
            bgImages.ToUniTaskAsyncEnumerable().ForEachAwaitAsync(async item =>
            {
                await item.Init();
            });
            
            await base.Init();
        }

        protected override void OnShow()
        {
            base.OnShow();
            
            textController.OnShow();
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

        public void SetChoices(IReadOnlyList<LocalizedString> choices, out IObservable<int> selected)
        {
            selected = null;
            
            if (choiceContainer != null)
            {
                choiceContainer.SetChoices(choices);

                selected = choiceContainer.OnChoicePressed;
            }
        }

        protected override void OnHide()
        {
            base.OnHide();
            
            bgImages.ForEach(item => item.Disable());
            
            censorController.Disable();
            textController.Disable();
            
            choiceContainer.Disable();
        }
    }
    
    public class DialogueSelectChoiceCanvas : DialogueSelectChoiceCanvas<DialogueSelectChoiceViewExecutor>
    {
    }
}